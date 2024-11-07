using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly ApplicationContext context;
        public UserRepository(ApplicationContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Users.Include("Appointments.Service")
                                .Where(w => w.Id == id)
                                .SelectMany(s => s.Appointments)
                                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetUserByBusinessAsync(long businessId)
        {
            return await context.Users.Include(i => i.BusinessUnity)
                                .Where(w => w.BusinessUnityId == businessId)
                                .ToListAsync();
        }

        public async Task<List<UserSchedule>> GetValidDaysSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            return await context.UserSchedules.Where(x => x.UserId == id && !x.IsRest)
                                              .ToListAsync();
        }

        public async Task<User> GetWithAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Users.Include("Appointments.Services.Service")
                                      .Include(i => i.Schedules)
                                      .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<long> GetIdByPhone(CancellationToken cancellationToken, string phone)
        {
            return await context.Users.Where(x => x.Phone == phone)
                                      .Select(s => s.Id)
                                      .FirstOrDefaultAsync();
        }

        public async Task<User> GetByPhoneWithBusinessUnitiesAsync(CancellationToken cancellationToken, string phone)
        {
            return await context.Users.Include(i => i.BusinessUnity.Company)
                                      .FirstOrDefaultAsync(w => w.Phone == phone);
        }
    }
}
