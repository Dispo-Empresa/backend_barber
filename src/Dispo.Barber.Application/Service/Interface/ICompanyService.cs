using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface ICompanyService
    {
        Task<long> CreateAsync(CancellationToken cancellationToken, CreateCompanyDTO companyDTO);

        Task<List<Company>> GetAllAsync(CancellationToken cancellationToken);

        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateCompanyDTO updateCompanyDTO);

        Task<Company> GetAsync(CancellationToken cancellationToken, long id);
    }
}
