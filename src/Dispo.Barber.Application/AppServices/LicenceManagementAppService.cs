using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Company;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class LicenceManagementAppService(ILogger<LicenceManagementAppService> logger,
                                             IUnitOfWork unitOfWork,
                                             ILicenceManagementService service) : ILicenceManagementAppService
    {
        public async Task ChangeLicensePlan(long companyId, ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.ChangeLicensePlan(companyId, changeLicensePlanDTO, cancellationToken));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error changing license plan");
                throw;
            }
        }
    }
}
