using Dispo.Barber.Domain.DTOs.Authentication;

namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface IAuthAppService
    {
        Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password);

        Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt);

        Task UpdatePurchaseTokenTeste(int userId, string purchaseToken, CancellationToken cancellationToken);
    }
}
