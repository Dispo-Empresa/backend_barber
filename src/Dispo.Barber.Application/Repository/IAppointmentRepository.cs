using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        Task<List<Appointment>> GetAppointmentByUserAndDateIdSync(CancellationToken cancellationToken, long userId, DateTime dateTimeSchedule);

        Task<List<Appointment>> GetFrequentAppointmentsByDaysBeforeAsync(CancellationToken cancellationToken, int daysBefore);

        Task<bool> CancelAllTodayAsync(CancellationToken cancellationToken, long userId);

        Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long userId);
    }
}
