using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IBusinessUnityRepository : IRepositoryBase<BusinessUnity>
    {
        Task<List<User>> GetUsersAsync(long id);

        Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id);
    }
}
