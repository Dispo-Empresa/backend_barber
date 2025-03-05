using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repositories
{
    public class BusinessUnityRepository : RepositoryBase<BusinessUnity>, IBusinessUnityRepository
    {
        private readonly ApplicationContext context;
        public BusinessUnityRepository(ApplicationContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await context.BusinessUnities.Where(w => w.Id == id)
                                .Include(i => i.Users)
                                .SelectMany(s => s.Users.Where(x => x.Status == UserStatus.Active || x.Status == UserStatus.Pending))
                                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await context.BusinessUnities.Where(w => w.Id == id)
                                .Include(i => i.Users)
                                .SelectMany(s => s.Users.Where(x => x.Status == UserStatus.Pending))
                                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetIdByCompanyAsync(long companyId)
        {
            return await context.BusinessUnities
                .Where(w => w.CompanyId == companyId)
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> GetCustomersAsync(CancellationToken cancellationToken, long id)
        {
            return await context.BusinessUnities
                .Where(bu => bu.Id == id)
                .SelectMany(bu => bu.Users)
                    .SelectMany(u => u.Appointments)
                        .Select(a => a.Customer)
                            .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetCompanyIdAsync(CancellationToken cancellationToken, long id)
        {
            return await context.BusinessUnities.Where(w => w.Id == id)
                                                .Select(s => s.CompanyId)
                                                .FirstOrDefaultAsync();
        }
    }
}
