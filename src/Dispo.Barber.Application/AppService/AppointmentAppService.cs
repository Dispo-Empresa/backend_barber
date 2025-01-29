using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
{
    public class AppointmentAppService(ILogger<AppointmentAppService> logger, IUnitOfWork unitOfWork, IAppointmentService service) : IAppointmentAppService
    {
        public async Task<Appointment> GetAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting appointments.");
                throw;
            }
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, createAppointmentDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating appointment.");
                throw;
            }
        }

        public async Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.InformProblemAsync(cancellationToken, id, informAppointmentProblemDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error informing appointment problem.");
                throw;
            }
        }

        public async Task CancelAppointmentAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CancelAppointmentAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error cancelling appointment.");
                throw;
            }
        }

        public async Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetScheduleConflictsAsync(cancellationToken, userId, startDate, endDate));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting appointments.");
                throw;
            }
        }

        public async Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetScheduleConflictsAsync(cancellationToken, userId, startTime, endTime, dayOfWeek));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting appointments.");
                throw;
            }
        }
    }
}
