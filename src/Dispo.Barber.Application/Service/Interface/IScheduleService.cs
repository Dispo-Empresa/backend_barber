using Dispo.Barber.Domain.DTO.Schedule;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IScheduleService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO);

        Task DeleteAsync(CancellationToken cancellationToken, long scheduleId);
    }
}
