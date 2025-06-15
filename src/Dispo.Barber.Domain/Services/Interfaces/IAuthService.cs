using Dispo.Barber.Domain.DTOs.Authentication.Request;
using Dispo.Barber.Domain.DTOs.Authentication.Response;

namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken);
        Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt);
    }
}
