using Dispo.Barber.Domain.DTOs.Hub;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Services.Interfaces
{
    public interface IHubLicenceValidationService
    {
        Task<LicenseDTO> GetOrCreateLicense(User user, CancellationToken cancellationToken);
    }
}
