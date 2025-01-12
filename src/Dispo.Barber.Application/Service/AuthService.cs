﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Authentication;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Domain.Exception;
using Microsoft.IdentityModel.Tokens;

namespace Dispo.Barber.Application.Service
{
    public class AuthService(IUserRepository repository) : IAuthService
    {
        public async Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password)
        {
            var user = await repository.GetByPhoneWithBusinessUnitiesAsync(cancellationToken, phone) ?? throw new NotFoundException("Usuário não encontrado.");
            if (!PasswordEncryptor.VerifyPassword(password, user.Password))
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            if (user.Status != UserStatus.Active)
            {
                throw new BusinessException("Usuário não está ativo.");
            }

            return CreateToken(user);
        }

        private AuthenticationResult CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            return new AuthenticationResult(tokenHandler.WriteToken(new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                claims: new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("phone", user.Phone),
                    new Claim("link", user.EntireSlug() ?? string.Empty),
                },
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            )), user);
        }
    }
}
