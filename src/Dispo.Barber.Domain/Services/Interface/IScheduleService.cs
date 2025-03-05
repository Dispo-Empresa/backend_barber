using Dispo.Barber.Domain.DTOs.Schedule;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IScheduleService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO);

        Task DeleteAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateScheduleDTO updateScheduleDTO);
    }
}
