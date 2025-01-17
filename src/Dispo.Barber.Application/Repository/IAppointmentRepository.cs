﻿using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IAppointmentRepository : IRepositoryBase<Appointment>
    {
        Task<List<Appointment>> GetAppointmentByUserAndDateIdSync(CancellationToken cancellationToken, long userId, DateTime dateTimeSchedule);

        Task<List<Appointment>> GetFrequentAppointmentsByDaysBeforeAsync(CancellationToken cancellationToken, int daysBefore);

        Task<bool> CancelAllByDateAsync(CancellationToken cancellationToken, long userId, DateTime date);

        Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long userId);

        Task<bool> CancelAllScheduledAsync(CancellationToken cancellationToken, long userId);

        Task<List<AppointmentDetailDTO>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate);
    }
}
