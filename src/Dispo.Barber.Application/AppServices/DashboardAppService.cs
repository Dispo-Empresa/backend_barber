using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.Models;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class DashboardAppService(ILogger<DashboardAppService> logger, IUnitOfWork unitOfWork, IDashboardService service) : IDashboardAppService
    {
        public async Task<Dashboard> BuildDashboardForUser(CancellationToken cancellationToken, long userId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.BuildDashboardForUser(cancellationToken, userId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error building dashboard.");
                throw;
            }
        }
    }
}
