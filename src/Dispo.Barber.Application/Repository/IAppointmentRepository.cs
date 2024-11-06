using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        Task<List<Appointment>> GetAppointmentByUserIdSync(CancellationToken cancellationToken, long userId);
        Task<List<Appointment>> GetAppointmentByUserAndDateIdSync(CancellationToken cancellationToken, long userId, DateTime dateTimeSchedule);
    }
}
