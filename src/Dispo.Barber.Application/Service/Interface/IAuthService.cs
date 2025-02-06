using Dispo.Barber.Domain.DTO.Authentication;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IAuthService
    {
        Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password);

        Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt);
    }
}
