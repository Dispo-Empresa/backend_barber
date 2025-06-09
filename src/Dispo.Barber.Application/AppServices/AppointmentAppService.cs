using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class AppointmentAppService(ILogger<AppointmentAppService> logger, 
                                       IUnitOfWork unitOfWork, 
                                       IAppointmentService service) : IAppointmentAppService
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

        public async Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO, bool notifyUsers = false, bool isChat = false)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, createAppointmentDTO, notifyUsers: notifyUsers, isChat : isChat));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating appointment.");
                throw;
            }
        }

        public async Task RescheduleAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.Reschedule(cancellationToken, createAppointmentDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error rescheduling appointment.");
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

        public async Task CancelAppointmentAsync(CancellationToken cancellationToken, long id, bool commit = true, bool notifyUsers = false)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CancelAppointmentAsync(cancellationToken, id, notifyUsers), commit);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error cancelling appointment.");
                throw;
            }
        }

        public async Task CancelAppointmentsAsync(CancellationToken cancellationToken, List<long> appointmentIds, bool commit = true)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CancelAppointmentsAsync(cancellationToken, appointmentIds), commit);
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

        public async Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek, bool isBreak)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetScheduleConflictsAsync(cancellationToken, userId, startTime, endTime, dayOfWeek, isBreak));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting appointments.");
                throw;
            }
        }
    }
}
