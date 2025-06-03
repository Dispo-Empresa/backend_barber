using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Authentication;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class AuthAppService(ILogger<AuthAppService> logger, 
                                IUnitOfWork unitOfWork, 
                                IAuthService service) : IAuthAppService
    {
        public async Task<AuthenticationResult> AuthenticateAsync(CancellationToken cancellationToken, string phone, string password)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.AuthenticateAsync(cancellationToken, phone, password), true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error authenticating.");
                throw;
            }
        }

        public async Task<AuthenticationResult> RefreshAuthenticationToken(CancellationToken cancellationToken, string refreshToken, string currentJwt)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.RefreshAuthenticationToken(cancellationToken, refreshToken, currentJwt), true, true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error refreshing token.");
                throw;
            }
        }
    }
}
