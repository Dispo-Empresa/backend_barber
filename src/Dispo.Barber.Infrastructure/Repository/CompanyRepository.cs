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

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Companies.Where(w => w.Id == id)
                                .Include(i => i.BusinessUnities)
                                .SelectMany(s => s.BusinessUnities)
                                .ToListAsync(cancellationToken);
        }

        public async Task<Company> GetWithBusinessUnitiesAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Companies.Include(i => i.BusinessUnities)
                                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
            this.context = context;
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(long id)
        {
            return await context.Companies.Where(w => w.Id == id)
                                .Include(i => i.BusinessUnities)
                                .SelectMany(s => s.BusinessUnities)
                                .ToListAsync();
        }

        public async Task<Company> GetWithBusinessUnitiesAsync(long id)
        {
            return await context.Companies.Include(i => i.BusinessUnities)
                                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<long>> GetServicesByCompanyAsync(long companyId)
        {
            var companyService = await context.CompanyServices
                            .Where(w => w.CompanyId == companyId)
                            .Select(w => w.ServiceId)
                            .ToListAsync();

            return companyService;
        }
    }
}
