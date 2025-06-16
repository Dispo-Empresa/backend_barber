using Dispo.Barber.Domain.DTOs.Schedule;

namespace Dispo.Barber.Application.AppServices.Interfaces
{
    public interface IScheduleAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO);

        Task DeleteAsync(CancellationToken cancellationToken, long scheduleId);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateScheduleDTO updateScheduleDTO);
    }
}
