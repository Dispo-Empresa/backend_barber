using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IUserAppService
    {
        Task CreateAsync(CreateUserDTO createUserDTO);

        Task AddServiceToUserAsync(long id, AddServiceToUserDTO addServiceToUserDTO);

        Task<List<Appointment>> GetUserAppointmentsAsync(long id, GetUserAppointmentsDTO getUserAppointmentsDTO);

        Task<List<UserSchedule>> GetUserSchedulesAsync(long id);

        Task DisableUserAsync(long id);

        Task UpdateAsync(long id, UpdateUserDTO updateUserDTO);
    }
}
