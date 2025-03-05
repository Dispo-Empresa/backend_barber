using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dispo.Barber.Application.Integration;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Authentication;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exception;
using Dispo.Barber.Domain.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Dispo.Barber.Application.Service
{
    public class AuthService(IUserRepository userRepository, ITokenRepository tokenRepository, IMemoryCache cache, IHubIntegration hubIntegration) : IAuthService
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

            var planType = await hubIntegration.GetPlanType(cancellationToken, user.BusinessUnity.CompanyId);
            var refreshToken = await GetOrCreateRefreshToken(cancellationToken, user);
            return BuildAuthenticationResult(user, refreshToken, planType);
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
            cache.Set(string.Format(BlacklistedJwtKey, currentJwt), true);

            var planType = await hubIntegration.GetPlanType(cancellationToken, user.BusinessUnity.CompanyId);
            return BuildAuthenticationResult(user, refreshToken, planType);
        }

        private AuthenticationResult BuildAuthenticationResult(User user, string refreshToken, PlanType planType)
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
                    new Claim("plan", planType.ToString()),
                ],
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            )), refreshToken, user, planType);
        }
    }
}
