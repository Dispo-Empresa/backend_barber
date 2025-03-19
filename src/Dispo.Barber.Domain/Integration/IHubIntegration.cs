using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Integration
{
    public interface IHubIntegration
    {
        Task<PlanType> GetPlanType(CancellationToken cancellationToken, long companyId);
        Task<LicenceDTO> GetLicenceDetails(CancellationToken cancellationToken, long companyId);
        Task CreateHubLicence(LicenceRequestDTO licenceRequestDTO, CancellationToken cancellationToken);
    }
}
