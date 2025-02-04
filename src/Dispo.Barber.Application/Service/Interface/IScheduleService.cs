using Dispo.Barber.Domain.DTO.Schedule;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IScheduleService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO);

        Task DeleteAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateScheduleDTO updateScheduleDTO);
    }
}
