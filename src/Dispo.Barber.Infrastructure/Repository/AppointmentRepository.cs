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
