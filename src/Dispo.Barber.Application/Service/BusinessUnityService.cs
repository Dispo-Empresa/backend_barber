using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service
{
    public class BusinessUnityService(IBusinessUnityRepository repository) : IBusinessUnityService
    {
        public async Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetUsersAsync(cancellationToken, id);
        }

        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetUsersAsync(cancellationToken, id);
        }
    }
}
