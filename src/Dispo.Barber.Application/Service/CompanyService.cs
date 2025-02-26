using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;

namespace Dispo.Barber.Application.Service
{
    public class CompanyService(IMapper mapper,
                                ICompanyRepository repository,
                                IServiceRepository serviceRepository,
                                IBusinessUnityRepository businessUnityRepository,
                                IBusinessUnityService businessUnityService) : ICompanyService
    {
        public async Task<long> CreateAsync(CancellationToken cancellationToken, CreateCompanyDTO companyDTO)
        {
            var company = mapper.Map<Company>(companyDTO);
            foreach (var serviceCompany in company.ServicesCompany)
            {
                await serviceRepository.AddAsync(cancellationToken, serviceCompany.Service);
            }

            company.Slug = company.Name.ToLowerInvariant().Replace(" ", "-");

            await repository.AddAsync(cancellationToken, company);
            await repository.SaveChangesAsync(cancellationToken);

            return company.Id;
        }

        public async Task<List<Company>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await repository.GetAllAsync(cancellationToken);
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetBusinessUnitiesAsync(cancellationToken, id);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateCompanyDTO updateCompanyDTO)
        {
            var company = await repository.GetWithBusinessUnitiesAsync(cancellationToken, id);
            if (company is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(updateCompanyDTO.Name))
            {
                company.Name = updateCompanyDTO.Name;
                company.Slug = company.Name.ToLowerInvariant().Replace(" ", "-");
            }

            if (company.BusinessUnities.Any())
            {
                if (!string.IsNullOrEmpty(updateCompanyDTO.Name))
                {
                    var businessUnity = company.BusinessUnities.First();
                    businessUnity.Phone = updateCompanyDTO.Phone;
                }
            }

            repository.Update(company);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<Company> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Empresa não existe.");
        }
    }
}
