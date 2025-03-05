using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repositories
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

        public async Task<IList<Service>> GetServicesByCompanyAsync(long id, bool? activated, CancellationToken cancellationToken)
        {
            return await context.CompanyServices
                                .Include(i => i.Company)
                                .Include(i => i.Service)
                                .Where(x => x.CompanyId == id && (activated == null || x.Service.Status == ServiceStatus.Active))
                                .Select(s => s.Service)
                                .ToListAsync(cancellationToken);
        }
    }
}
