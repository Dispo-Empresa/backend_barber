using Dispo.Barber.Domain.DTO.Service;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IServiceAppService
    {
        Task CreateAsync(CreateServiceDTO createServiceDTO);
    }
}
