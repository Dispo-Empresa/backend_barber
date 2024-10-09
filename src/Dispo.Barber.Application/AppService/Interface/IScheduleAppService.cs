using Dispo.Barber.Domain.DTO.Schedule;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IScheduleAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO);

        Task DeleteAsync(CancellationToken cancellationToken, long scheduleId);
    }
}
