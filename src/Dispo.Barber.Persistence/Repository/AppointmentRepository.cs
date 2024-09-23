using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Persistence.Context;

namespace Dispo.Barber.Persistence.Repository
{
    public class AppointmentRepository : RepositoryBase<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
