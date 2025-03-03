using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Repositories
{
    public interface ICompanyRepository : IRepositoryBase<Company>
    {
        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task<Company> GetWithBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task<List<long>> GetServicesByCompanyAsync(long companyId);

        Task<Company> GetWithServicesAsync(CancellationToken cancellationToken, long id);
    }
}
