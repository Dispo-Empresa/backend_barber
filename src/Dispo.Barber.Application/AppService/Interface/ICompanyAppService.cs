using Dispo.Barber.Domain.DTO.Company;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface ICompanyAppService
    {
        Task CreateAsync(CreateCompanyDTO companyDTO);
    }
}
