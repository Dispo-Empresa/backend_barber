using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
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
