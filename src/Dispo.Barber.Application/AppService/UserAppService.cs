using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
{
    public class UserAppService(ILogger<UserAppService> logger, IUnitOfWork unitOfWork, IUserService service) : IUserAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, createUserDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating user.");
                throw;
            }
        }

        public async Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, List<long> services)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.AddServiceToUserAsync(cancellationToken, id, services));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUserAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user appointments.");
                throw;
            }
        }

        public async Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUserSchedulesAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user schedules.");
                throw;
            }
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.UpdateAsync(cancellationToken, id, updateUserDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error updating user.");
                throw;
            }
        }

        public async Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.ChangeStatusAsync(cancellationToken, id, changeStatusDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error changing user status.");
                throw;
            }
        }

        public async Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.ChangePasswordAsync(cancellationToken, id, changePasswordDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error changing password.");
                throw;
            }
        }

        public async Task<long> GetUserIdByPhone(CancellationToken cancellationToken, string phone)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUserIdByPhone(cancellationToken, phone));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user ID.");
                throw;
            }
        }
    }
}