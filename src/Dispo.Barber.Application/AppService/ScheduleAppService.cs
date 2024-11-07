using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Schedule;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
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

        public async Task DeleteAsync(CancellationToken cancellationToken, long scheduleId)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.DeleteAsync(cancellationToken, scheduleId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error deleting schedule.");
                throw;
            }
        }
    }
}
