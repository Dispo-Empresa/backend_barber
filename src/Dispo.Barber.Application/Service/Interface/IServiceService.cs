using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IServiceService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO);

        Task<IList<ServiceInformationDTO>> GetServicesList(CancellationToken cancellationToken, long companyId);

        Task<IList<ServiceInformationDTO>> GetAllServicesList(CancellationToken cancellationToken);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateServiceDTO updateServiceDTO);

        Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ServiceStatus status);
    }
}
