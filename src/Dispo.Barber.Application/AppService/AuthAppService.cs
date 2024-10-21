using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;
using Microsoft.IdentityModel.Tokens;

namespace Dispo.Barber.Application.AppService
{
    public class AuthAppService(IUnitOfWork unitOfWork) : IAuthAppService
    {
        public async Task<string> Authenticate(CancellationToken cancellationToken, string phone, string password)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var userRepository = unitOfWork.GetRepository<IUserRepository>();
                var user = await userRepository.GetFirstAsync(cancellationToken, w => w.Phone == phone);
                if (user == null)
                {
                    throw new NotFoundException("Usuário não encontrado.");
                }

                if (!PasswordEncryptor.VerifyPassword(password, user.Password))
                {
                    throw new NotFoundException("Usuário não encontrado.");
                }

                return CreateToken(user);
            });
        }

        private string CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("phone", user.Phone),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            ));
        }
    }
}
