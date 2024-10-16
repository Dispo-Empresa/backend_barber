using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface ICompanyRepository : IRepositoryBase<Company>
    {
        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task<Company> GetWithBusinessUnitiesAsync(CancellationToken cancellationToken, long id);
    }
}
