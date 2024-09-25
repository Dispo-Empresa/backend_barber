using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.Entities;

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
    }
}
