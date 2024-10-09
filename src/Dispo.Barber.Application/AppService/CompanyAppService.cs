using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;

namespace Dispo.Barber.Application.AppService
{
    public class CompanyAppService(IUnitOfWork unitOfWork, IMapper mapper) : ICompanyAppService
    {
        public async Task CreateAsync(CreateCompanyDTO companyDTO)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                var company = mapper.Map<Company>(companyDTO);
                await companyRepository.AddAsync(company);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }

        public async Task<List<Company>> GetAllAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                return await companyRepository.GetAllAsync();
            });
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(long id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                return await companyRepository.GetBusinessUnitiesAsync(id);
            });
        }

        public async Task UpdateAsync(long id, UpdateCompanyDTO updateCompanyDTO)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                var company = await companyRepository.GetWithBusinessUnitiesAsync(id);
                if (company is null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(updateCompanyDTO.Name))
                {
                    company.Name = updateCompanyDTO.Name;
                }

                if (company.BusinessUnities.Any())
                {
                    if (!string.IsNullOrEmpty(updateCompanyDTO.Name))
                    {
                        var businessUnity = company.BusinessUnities.First();
                        businessUnity.Phone = updateCompanyDTO.Phone;
                    }
                }

                companyRepository.Update(company);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }

        public async Task<Company> GetAsync(long id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                var company = await companyRepository.GetAsync(id);
                if (company is null)
                {
                    throw new NotFoundException("Empresa não existe.");
                }

                return company;
            });
        }
    }
}
