using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IBusinessUnityRepository : IRepositoryBase<BusinessUnity>
    {
        Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id);

        Task<long> GetIdByCompanyAsync(long companyId);

        Task<List<Customer>> GetCustomersAsync(CancellationToken cancellationToken, long id);
    }
}
