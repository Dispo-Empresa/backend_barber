using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IServiceAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO);

        Task<IList<ServiceInformationDTO>> GetServicesList(CancellationToken cancellationToken, long companyId);

        Task<IList<ServiceInformationDTO>> GetAllServicesList(CancellationToken cancellationToken);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateServiceDTO updateServiceDTO);

        Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ServiceStatus status);
    }
}
