using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
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
                                .SelectMany(s => s.Users.Where(x => x.Status == UserStatus.Active))
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
            var businessUnity = await context.BusinessUnities
                .Where(w => w.CompanyId == companyId)
                .Select(b => b.Id)
                .FirstOrDefaultAsync();

            return businessUnity;
        }

    }
}
