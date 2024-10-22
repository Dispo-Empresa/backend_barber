using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class ServiceRepository : RepositoryBase<Service>, IServiceRepository
    {
        private readonly ApplicationContext context;
        public ServiceRepository(ApplicationContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<List<Service>> GetListServiceAsync(List<long> serviceIds)
        {
            if (serviceIds == null || !serviceIds.Any())
            {
                return new List<Service>();
            }

            var services = await context.Services
                .Where(service => serviceIds.Contains(service.Id))
                .ToListAsync();

            return services; 
        }

        public async Task<IList<Service>> GetServicesByCompanyAsync(long id, CancellationToken cancellationToken)
        {
            return await context.CompanyServices
                                .Include(i => i.Company)
                                .Include(i => i.Service)
                                .Where(x => x.CompanyId == id)
                                .Select(s => s.Service)
                                .ToListAsync(cancellationToken);
        }
    }
}
