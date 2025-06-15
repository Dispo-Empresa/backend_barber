using Dispo.Barber.Domain.DTOs.Authentication.Request;
using Dispo.Barber.Domain.DTOs.Authentication.Response;
using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration.HubClient;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;
using Dispo.Barber.Domain.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dispo.Barber.Domain.Services
{
    public class AuthService(IUserRepository userRepository,
                             ITokenRepository tokenRepository,
                             IBlacklistService blacklistService,
                             IHubIntegration hubIntegration,
                             IHubLicenceValidationService hubLicenceValidationService,
                             ISubscriptionValidationService subscriptionValidationService) : IAuthService
    {
        public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByPhoneWithBusinessUnitiesAsync(cancellationToken, request.Phone)
                ?? throw new NotFoundException("Usuário não encontrado.");

            ValidateUser(user, request.Password);

            var licenseDetails = await hubLicenceValidationService.GetOrCreateLicense(user, cancellationToken);

            var subscriptionData = await ProcessSubscriptionDataAsync(user, licenseDetails, request.Platform, cancellationToken);

            if (licenseDetails.Plan.Id != (int)PlanType.BarberPremium ||
               (licenseDetails.Plan.Id == (int)PlanType.BarberPremium && subscriptionData!.IsSubscriptionValid))
            {
                ChangePlataformDeviceToken(user, request.Platform, request.DeviceToken);
            }

            return await BuildAuthenticationResult(user, subscriptionData, cancellationToken);
        }

        public async Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt)
        {
            var token = await tokenRepository.GetFirstAsync(cancellationToken, w => w.RefreshToken == refreshToken.ToString())
                ?? throw new NotFoundException("Token não encontrado");

            if (token.ExpirationDate <= LocalTime.Now)
            {
                tokenRepository.Delete(token);
                await tokenRepository.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedAccessException("Token expirado.");
            }

            var user = await userRepository.GetByIdWithBusinessUnitiesAsync(cancellationToken, token.UserId)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (user.Status != UserStatus.Active)
            {
                throw new BusinessException("Usuário não está ativo.");
            }

            blacklistService.PutInBlacklist(currentJwt);

            var licenseDetails = await hubIntegration.GetLicenseDetails(cancellationToken, user.BusinessUnity.CompanyId)
                ?? throw new NotFoundException("Licença não encontrada. Por favor, tente mais tarde.");

            var subscriptionData = await ProcessSubscriptionDataAsync(user, licenseDetails, null, cancellationToken);

            return await BuildAuthenticationResult(user, subscriptionData, cancellationToken);
        }

        private async Task<string> GetOrCreateRefreshToken(CancellationToken cancellationToken, User user)
        {
            var existingToken = await tokenRepository.GetFirstAsync(cancellationToken, w => w.UserId == user.Id);
            if (existingToken != null)
            {
                if (existingToken.ExpirationDate >= LocalTime.Now)
                {
                    return existingToken.RefreshToken;
                }

                tokenRepository.Delete(existingToken);
                await tokenRepository.SaveChangesAsync(cancellationToken);
            }

            var refreshToken = Guid.NewGuid().ToString();
            await tokenRepository.AddAsync(cancellationToken, new Token
            {
                RefreshToken = refreshToken,
                UserId = user.Id,
                ExpirationDate = LocalTime.Now.AddDays(7),
            });

            await tokenRepository.SaveChangesAsync(cancellationToken);

            return refreshToken;
        }

        private void ValidateUser(User user, string password)
        {
            if (!PasswordEncryptor.VerifyPassword(password, user.Password))
                throw new NotFoundException("Senha inválida.");

            if (user.Status != UserStatus.Active)
                throw new BusinessException("Usuário não está ativo.");
        }

        private async Task<SubscriptionData> ProcessSubscriptionDataAsync(User user, LicenseDTO licenseDetails, DevicePlatform? platform, CancellationToken cancellationToken)
        {
            if (licenseDetails.Plan.IsPremiumPlan())
            {
                var subscriptionData = await subscriptionValidationService.ValidateSubscriptionAsync(user, platform, cancellationToken);
                subscriptionData.Plan = licenseDetails.Plan;

                return subscriptionData;
            }
            else if (licenseDetails.Plan.IsTrial())
            {
                return new SubscriptionData
                {
                    ExpirationDate = licenseDetails.ExpirationDate,
                    Plan = licenseDetails.Plan,
                };
            }
            else
            {
                return new SubscriptionData
                {
                    Plan = licenseDetails.Plan
                };
            }
        }

        private void ChangePlataformDeviceToken(User user, DevicePlatform currentPlataform, string currentDeviceToken)
        {
            if (user.DeviceToken == currentDeviceToken && user.Platform == currentPlataform)
                return;

            user.Platform = currentPlataform;
            user.DeviceToken = currentDeviceToken;

            userRepository.Update(user);
        }

        private async Task<AuthenticationResult> BuildAuthenticationResult(User user, SubscriptionData subscription, CancellationToken cancellationToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();

            var refreshToken = await GetOrCreateRefreshToken(cancellationToken, user);
            var token = tokenHandler.WriteToken(new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                claims:
                [
                    new Claim("id", user.Id.ToString()),
                    new Claim("role", user.Role.ToString()),
                ],
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            ));

            return new AuthenticationResult(token, refreshToken, user, subscription);
        }
    }
}
