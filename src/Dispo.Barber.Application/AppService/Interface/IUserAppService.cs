using Dispo.Barber.Domain.DTO.User;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IUserAppService
    {
        Task AddServiceToUserAsync(long id, AddServiceToUserDTO addServiceToUserDTO);
    }
}
