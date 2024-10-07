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

        public async Task<List<long>> GetServicesByCompanyAsync(long companyId)
        {
            var companyService = await context.CompanyServices
                            .Where(w => w.CompanyId == companyId)
                            .Select(w=> w.ServiceId)
                            .ToListAsync();

            return companyService;
        }
    }
}
