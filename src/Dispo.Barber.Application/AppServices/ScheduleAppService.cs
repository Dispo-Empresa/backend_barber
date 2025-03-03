using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class ScheduleAppService(ILogger<ScheduleAppService> logger, IUnitOfWork unitOfWork, IScheduleService service) : IScheduleAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, createScheduleDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating schedule.");
                throw;
            }
        }

        public async Task DeleteAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.DeleteAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error deleting schedule.");
                throw;
            }
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateScheduleDTO updateScheduleDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.UpdateAsync(cancellationToken, id, updateScheduleDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error deleting schedule.");
                throw;
            }
        }
    }
}
