using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Infrastructure.Context;

namespace Dispo.Barber.Infrastructure.Repositories
{
    public class ServiceAppointmentRepository : RepositoryBase<ServiceAppointment>, IServiceAppointmentRepository
    {
        private readonly ApplicationContext _context;
        public ServiceAppointmentRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }
    }
}
