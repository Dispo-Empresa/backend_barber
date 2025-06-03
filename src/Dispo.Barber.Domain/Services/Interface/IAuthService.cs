using Dispo.Barber.Domain.DTOs.Authentication;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password);
        Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt);
    }
}
