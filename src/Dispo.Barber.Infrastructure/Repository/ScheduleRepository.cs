using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class ScheduleRepository : RepositoryBase<UserSchedule>, IScheduleRepository
    {
        public ScheduleRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
