using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface ICompanyAppService
    {
        Task CreateAsync(CreateCompanyDTO companyDTO);

        Task<List<Company>> GetAllAsync();

        Task<List<BusinessUnity>> GetBusinessUnitiesAsync(long id);
    }
}
