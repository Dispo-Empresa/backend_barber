using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Integration
{
    public interface IHubIntegration
    {
        Task<PlanType> GetPlanType(CancellationToken cancellationToken, long companyId);
        Task<LicenseDTO?> GetLicenseDetails(CancellationToken cancellationToken, long companyId);
        Task<LicenseDTO> CreateHubLicense(LicenseRequestDTO licenceRequestDTO, CancellationToken cancellationToken);
    }
}
