using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface IAppointmentAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO);

        Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO);

        Task<Appointment> GetAsync(CancellationToken cancellationToken, long id);

        Task CancelAppointmentAsync(CancellationToken cancellationToken, long id, bool commit = true);

        Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate);

        Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek, bool isBreak);
    }
}
