﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dispo.Barber.Domain.DTOs.Authentication;
using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Integration;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;
using Microsoft.IdentityModel.Tokens;

namespace Dispo.Barber.Domain.Services
{
    public class AuthService(IUserRepository userRepository, ITokenRepository tokenRepository, IBlacklistService blacklistService, IHubIntegration hubIntegration) : IAuthService
    {
        private const string BlacklistedJwtKey = "blacklisted-{0}";

        public async Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password)
        {
            var user = await userRepository.GetByPhoneWithBusinessUnitiesAsync(cancellationToken, phone) ?? throw new NotFoundException("Usuário não encontrado.");
            if (!PasswordEncryptor.VerifyPassword(password, user.Password))
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            if (user.Status != UserStatus.Active)
            {
                throw new BusinessException("Usuário não está ativo.");
            }

            var licenceDetails = await hubIntegration.GetLicenceDetails(cancellationToken, user.BusinessUnity.CompanyId);
            var refreshToken = await GetOrCreateRefreshToken(cancellationToken, user);
            return BuildAuthenticationResult(user, refreshToken, licenceDetails);
        }

        public async Task<string> GetOrCreateRefreshToken(CancellationToken cancellationToken, User user)
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

        public async Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt)
        {
            var token = await tokenRepository.GetFirstAsync(cancellationToken, w => w.RefreshToken == refreshToken.ToString()) ?? throw new NotFoundException("Token não encontrado");
            if (token.ExpirationDate <= LocalTime.Now)
            {
                tokenRepository.Delete(token);
                await tokenRepository.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedAccessException("Token expirado.");
            }

            var user = await userRepository.GetByIdWithBusinessUnitiesAsync(cancellationToken, token.UserId) ?? throw new NotFoundException("Usuário não encontrado.");
            if (user.Status != UserStatus.Active)
            {
                throw new BusinessException("Usuário não está ativo.");
            }

            blacklistService.PutInBlacklist(currentJwt);
            var licenceDetails = await hubIntegration.GetLicenceDetails(cancellationToken, user.BusinessUnity.CompanyId);
            return BuildAuthenticationResult(user, refreshToken, licenceDetails);
        }

        private AuthenticationResult BuildAuthenticationResult(User user, string refreshToken, LicenceDTO licenceDetails)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            return new AuthenticationResult(tokenHandler.WriteToken(new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                claims:
                [
                    new Claim("id", user.Id.ToString()),
                    new Claim("phone", user.Phone),
                    new Claim("link", user.EntireSlug() ?? string.Empty),
                    new Claim("plan", licenceDetails.Plan.Id.ToString()),
                ],
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            )), refreshToken, user, licenceDetails);
        }
    }
}
