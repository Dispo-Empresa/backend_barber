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
        public async Task CreateAsync(CancellationToken cancellationToken, CreateCompanyDTO companyDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                var company = mapper.Map<Company>(companyDTO);
                await companyRepository.AddAsync(cancellationToken, company);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task<List<Company>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                return await companyRepository.GetAllAsync(cancellationToken);
            });
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                return await companyRepository.GetBusinessUnitiesAsync(cancellationToken, id);
            });
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateCompanyDTO updateCompanyDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                var company = await companyRepository.GetWithBusinessUnitiesAsync(cancellationToken, id);
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
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task<Company> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                var company = await companyRepository.GetAsync(cancellationToken, id);
                if (company is null)
                {
                    throw new NotFoundException("Empresa não existe.");
                }

                return company;
            });
        }

        public async Task<List<Company>> GetAllAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                return await companyRepository.GetAllAsync(cancellationTokenSource.Token);
            });
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(long id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var companyRepository = unitOfWork.GetRepository<ICompanyRepository>();
                return await companyRepository.GetBusinessUnitiesAsync(cancellationTokenSource.Token, id);
            });
        }
    }
}
