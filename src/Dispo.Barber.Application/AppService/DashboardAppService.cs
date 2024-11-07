using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
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
