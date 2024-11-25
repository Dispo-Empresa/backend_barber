using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.User;
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

        public async Task<List<Appointment>> GetAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            return await context.Appointments.Include("Services.Service")
                                .Where(w => w.AcceptedUserId == id && w.Date >= getUserAppointmentsDTO.StartDate && w.Date <= getUserAppointmentsDTO.EndDate)
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
            return await context.Users.Include("BusinessUnity.Company")
                                      .FirstOrDefaultAsync(w => w.Phone == phone);
        }

        public async Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug)
        {
            return await context.Users.Include("BusinessUnity.Company")
                                      .FirstOrDefaultAsync(w => w.Slug == userSlug && w.BusinessUnity != null && w.BusinessUnity.Company != null && w.BusinessUnity.Company.Slug == companySlug);
        }
    }
}
