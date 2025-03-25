using Dispo.Barber.Domain.DTOs.Company;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface ICompanyService
    {
        Task<long> CreateAsync(CancellationToken cancellationToken, CreateCompanyDTO companyDTO);

        Task<List<Company>> GetAllAsync(CancellationToken cancellationToken);

        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateCompanyDTO updateCompanyDTO);

        Task<Company> GetAsync(CancellationToken cancellationToken, long id);

        Task UpdateOwner(CancellationToken cancellationToken, long id, long ownerId);
    }
}
