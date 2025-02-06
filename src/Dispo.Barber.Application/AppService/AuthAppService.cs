using System.Numerics;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Authentication;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
{
    public class AuthAppService(ILogger<AuthAppService> logger, IUnitOfWork unitOfWork, IAuthService service) : IAuthAppService
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
