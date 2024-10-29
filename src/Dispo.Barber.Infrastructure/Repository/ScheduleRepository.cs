using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class ScheduleRepository : RepositoryBase<UserSchedule>, IScheduleRepository
    {
        private readonly ApplicationContext _context;
        public ScheduleRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserSchedule>> GetScheduleByUserId(long userId)
        {
            return await _context.UserSchedules.Where(x=> x.UserId == userId && x.IsRest.Equals(false) && x.DayOff.Equals(false))
                                               .ToListAsync();
        }
    }
}
