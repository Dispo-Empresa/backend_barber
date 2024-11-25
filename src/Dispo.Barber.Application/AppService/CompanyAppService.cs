using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Company;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
{
    public class CompanyAppService(ILogger<CompanyAppService> logger, IUnitOfWork unitOfWork, ICompanyService service, IUserService userService) : ICompanyAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateCompanyDTO companyDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, companyDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating company.");
                throw;
            }
        }

        public async Task<List<Company>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetAllAsync(cancellationToken));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting all companies.");
                throw;
            }
        }

        public async Task<List<BusinessUnity>> GetBusinessUnitiesAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetBusinessUnitiesAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting company business unities.");
                throw;
            }
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateCompanyDTO updateCompanyDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.UpdateAsync(cancellationToken, id, updateCompanyDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error updating company.");
                throw;
            }
        }

        public async Task<Company> GetAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting companies.");
                throw;
            }
        }

        public async Task<List<UserDTO>> GetUsersAsync(CancellationToken cancellationToken, long companyId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await userService.GetByCompanyId(cancellationToken, companyId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user by ID.");
                throw;
            }
        }
    }
}
