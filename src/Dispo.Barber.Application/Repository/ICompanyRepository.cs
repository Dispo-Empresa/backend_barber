using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface ICompanyRepository : IRepositoryBase<Company>
    {
        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(long id);
    }
}
