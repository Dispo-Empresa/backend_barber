using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
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
                .Where(x => x.AcceptedUserId == userId
                            && x.Date.Date == dateTimeSchedule.Date
                            && x.Status != AppointmentStatus.Completed)
                .ToListAsync(cancellationToken);

            return appointment;
        }
    }
}
