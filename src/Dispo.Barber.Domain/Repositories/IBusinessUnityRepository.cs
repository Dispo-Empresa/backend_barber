using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Repositories
{
    public interface IBusinessUnityRepository : IRepositoryBase<BusinessUnity>
    {
        Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id);

        Task<long> GetIdByCompanyAsync(long companyId);

        Task<long> GetCompanyIdAsync(CancellationToken cancellationToken, long id);

        Task<List<Customer>> GetCustomersAsync(CancellationToken cancellationToken, long id);
    }
}
