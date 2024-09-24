using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IUserAppService
    {
        Task AddServiceToUserAsync(long id, AddServiceToUserDTO addServiceToUserDTO);

        Task<List<Appointment>> GetUserAppointmentsAsync(long id, GetUserAppointmentsDTO getUserAppointmentsDTO);
    }
}
