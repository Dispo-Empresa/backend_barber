using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Persistence.Context;

namespace Dispo.Barber.Persistence.Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
