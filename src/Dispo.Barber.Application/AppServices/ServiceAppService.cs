using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class ServiceAppService(ILogger<ServiceAppService> logger, IUnitOfWork unitOfWork, IServiceService service) : IServiceAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateServiceDTO createServiceDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, createServiceDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating service.");
                throw;
            }
        }

        public async Task<IList<ServiceInformationDTO>> GetServicesList(CancellationToken cancellationToken, long companyId, bool? activated)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetServicesList(cancellationToken, companyId, activated));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting services.");
                throw;
            }
        }

        public async Task<IList<ServiceInformationDTO>> GetAllServicesList(CancellationToken cancellationToken)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetAllServicesList(cancellationToken));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting services.");
                throw;
            }
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateServiceDTO updateServiceDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.UpdateAsync(cancellationToken, id, updateServiceDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error updating service.");
                throw;
            }
        }

        public async Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ServiceStatus status)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.ChangeStatusAsync(cancellationToken, id, status));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error inactivating service.");
                throw;
            }
        }

    }
}
