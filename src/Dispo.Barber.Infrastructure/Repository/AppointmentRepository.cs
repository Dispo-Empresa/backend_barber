using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class AppointmentRepository : RepositoryBase<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationContext context;
        public AppointmentRepository(ApplicationContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Appointment>> GetFrequentAppointmentsByDaysBeforeAsync(CancellationToken cancellationToken, int daysBefore)
        {
            DateTime referenceDate = DateTime.Now;
            var startDate = referenceDate.AddDays(-daysBefore);

            // Obter agendamentos dentro do intervalo
            var appointments = await context.Appointments
                .Include(a => a.AcceptedUser)
                .Include("Services.Service.UserServices")
                .Include(a => a.Customer) // Incluímos os dados dos clientes
                .Where(a => a.Date.Date >= startDate.Date
                            && a.Date.Date <= referenceDate.Date
                            /*&& a.Status == AppointmentStatus.Completed*/)
                .ToListAsync(cancellationToken);

            // Filtrar apenas os agendamentos de clientes que têm mais de um agendamento
            var distinctAppointments = appointments
             .GroupBy(a => a.Customer.Id) // Agrupamos por ID do cliente
             .Select(group => group.OrderByDescending(a => a.Date).First()) // Pegamos o agendamento mais recente de cada grupo
             .ToList();

            return distinctAppointments;
        }

        public async Task<List<Appointment>> GetAppointmentByUserAndDateIdSync(CancellationToken cancellationToken, long userId, DateTime dateTimeSchedule)
        {
            dateTimeSchedule = DateTime.SpecifyKind(dateTimeSchedule, DateTimeKind.Utc);

            var appointment = await context.Appointments
                .Include(a => a.Services)
                .ThenInclude(s => s.Service)
                .Where(w => w.AcceptedUserId == userId
                            && w.Date.Date == dateTimeSchedule.Date
                            && w.Status != AppointmentStatus.Completed)
                .ToListAsync(cancellationToken);

            return appointment;
        }

        public async Task<bool> CancelAllByDateAsync(CancellationToken cancellationToken, long userId, DateTime date)
        {
            return await context.Appointments
                .Where(w => w.AcceptedUserId == userId && w.Date.Date == date.Date)
                .ExecuteUpdateAsync(set => set.SetProperty(a => a.Status, AppointmentStatus.Canceled), cancellationToken) > 0;
        }

        public async Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long userId)
        {
            return await context.Appointments.Include("Services.Service").Include("Customer")
                .Where(w => w.AcceptedUserId == userId && w.Date >= LocalTime.Now.Date && w.Status == AppointmentStatus.Scheduled)
                .OrderBy(o => o.Date)
                .Take(10)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CancelAllScheduledAsync(CancellationToken cancellationToken, long userId)
        {
            return await context.Appointments
                .Where(w => w.AcceptedUserId == userId && w.Status == AppointmentStatus.Scheduled)
                .ExecuteUpdateAsync(set => set.SetProperty(a => a.Status, AppointmentStatus.Canceled), cancellationToken) > 0;
        }

        public async Task<bool> CancelAllUserScheduledByDateAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate)
        {
            return await context.Appointments
                .Where(w => w.AcceptedUserId == userId && w.Date >= startDate && w.Date <= endDate && w.Status == AppointmentStatus.Scheduled)
                .ExecuteUpdateAsync(set => set.SetProperty(a => a.Status, AppointmentStatus.Canceled), cancellationToken) > 0;
        }

        public async Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate)
        {
            return await context.Appointments
                                .Include(a => a.AcceptedUser)
                                .Include("Services.Service.UserServices")
                                .Include(a => a.Customer) // Incluímos os dados dos clientes
                                .Where(w => w.AcceptedUserId == userId && w.Date >= startDate && w.Date <= endDate && w.Status == AppointmentStatus.Scheduled)
                                .OrderBy(o => o.Date)
                                .ToListAsync(cancellationToken);
        }
    }
}
