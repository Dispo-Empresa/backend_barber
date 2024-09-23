using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Persistence.Context;

namespace Dispo.Barber.Persistence.Repository
{
    public class ServiceRepository : RepositoryBase<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationContext context) 
            : base(context)
        {
        }
    }
}
