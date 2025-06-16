using Dispo.Barber.Application.AppServices.Interfaces;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class BusinessUnityAppService(ILogger<BusinessUnityAppService> logger, 
                                         IUnitOfWork unitOfWork, 
                                         IBusinessUnityService service,
                                         ICustomerRepository customerRepository) : IBusinessUnityAppService
    {
        public async Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUsersAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting users from business unity.");
                throw;
            }
        }

        public async Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetPendingUsersAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting pending users from business unity");
                throw;
            }
        }

        public async Task<List<CustomerDetailDTO>> GetBusinessUnityCustomersAsync(CancellationToken cancellationToken, long businessUnityId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await customerRepository.GetBusinessUnityCustomersAsync(cancellationToken, businessUnityId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user by ID.");
                throw;
            }
        }
    }
}
