using Dispo.Barber.Domain.DTO.Service;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IServiceService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO);

        Task<IList<ServiceListDTO>> GetServicesList(CancellationToken cancellationToken, long companyId);

        Task<IList<ServiceListDTO>> GetAllServicesList(CancellationToken cancellationToken);
    }
}
