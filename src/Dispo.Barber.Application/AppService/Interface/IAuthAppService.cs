using Dispo.Barber.Domain.DTO.Authentication;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IAuthAppService
    {
        Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password);
    }
}
