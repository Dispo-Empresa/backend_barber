using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IAppointmentService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO, bool notifyUsers = false);

        Task Reschedule(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO);

        Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO);

        Task<Appointment> GetAsync(CancellationToken cancellationToken, long id);

        Task CancelAppointmentAsync(CancellationToken cancellationToken, long id, bool notifyUsers = false);

        Task CancelAppointmentsAsync(CancellationToken cancellationToken, List<long> appointmentIds);

        Task CancelAllByDateAsync(CancellationToken cancellationToken, long userId, DateTime date);

        Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long userId);

        Task CancelAllScheduledAsync(CancellationToken cancellationToken, long userId);

        Task CancelAllUserScheduledByDateAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate);

        Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate);
        Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek, bool isBreak);
    }
}
