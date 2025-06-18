using Dispo.Barber.Domain.DTOs.Company;

namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface ILicenceManagementService
    {
        Task ChangeLicensePlan(long companyId, ChangeLicensePlanDTO changeLicensePlanDTO, CancellationToken cancellationToken);
    }
}
