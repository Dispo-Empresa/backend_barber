using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Integration.HubClient
{
    public interface IHubIntegration
    {
        Task<LicensePlan> GetPlanType(CancellationToken cancellationToken, long companyId);
        Task<LicenseDTO?> GetLicenseDetails(CancellationToken cancellationToken, long companyId);
        Task<LicenseDTO> CreateHubLicense(LicenseRequestDTO licenceRequestDTO, CancellationToken cancellationToken);
    }
}
