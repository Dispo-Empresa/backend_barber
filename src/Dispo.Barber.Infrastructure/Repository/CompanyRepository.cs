using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        private readonly ApplicationContext context;
        public CompanyRepository(ApplicationContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(long id)
        {
            return await context.Companies.Where(w => w.Id == id)
                                .Include(i => i.BusinessUnities)
                                .SelectMany(s => s.BusinessUnities)
                                .ToListAsync();
        }
    }
}
