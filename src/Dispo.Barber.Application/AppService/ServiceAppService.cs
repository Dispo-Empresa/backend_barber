using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
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

        public async Task<IList<ServiceListDTO>> GetServicesList(CancellationToken cancellationToken, long companyId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetServicesList(cancellationToken, companyId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting services.");
                throw;
            }
        }

        public async Task<IList<ServiceListDTO>> GetAllServicesList(CancellationToken cancellationToken)
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
                logger.LogError(e, "Error getting services.");
                throw;
            }
        }
    }
}
