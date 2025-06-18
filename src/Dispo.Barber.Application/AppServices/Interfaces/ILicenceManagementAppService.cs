using Dispo.Barber.Domain.DTOs.Company;

namespace Dispo.Barber.Application.AppServices.Interfaces
{
    public interface ILicenceManagementAppService
    {
        Task ChangeLicensePlan(long companyId, ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken);
    }
}
