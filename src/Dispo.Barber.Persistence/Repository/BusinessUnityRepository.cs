using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Persistence.Context;

namespace Dispo.Barber.Persistence.Repository
{
    public class BusinessUnityRepository : RepositoryBase<BusinessUnity>, IBusinessUnityRepository
    {
        public BusinessUnityRepository(ApplicationContext context) 
            : base(context)
        {
        }
    }
}
