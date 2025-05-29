using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class UserAppService(ILogger<UserAppService> logger,
                               IUnitOfWork unitOfWork,
                               IUserService service,
                               ICustomerService customerService,
                               IAppointmentService appointmentService,
                               IUserRepository userRepository) : IUserAppService
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
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var appointments = await service.GetUserAppointmentsAsync(cancellationToken, id, getUserAppointmentsDTO);

                    // Estamos completando os agendamentos que já passaram do horário porem estamos fazendo um update em um metodo get, vamos ter que melhorar isso
                    foreach (var appointment in appointments.Where(w => w.Date <= LocalTime.Now && w.Status == AppointmentStatus.Scheduled).ToList())
                    {
                        var duration = appointment.Services.Select(w => w.Service).Sum(w => w.Duration);
                        if (appointment.Date.AddMinutes(duration) >= LocalTime.Now)
                            continue;

                        appointment.Status = AppointmentStatus.Completed;
                    }
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    return appointments;
                }, true);
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

        public async Task<List<UserSchedule>> GetUserBreaksAsync(CancellationToken cancellationToken, long id, DayOfWeek dayOfWeek)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUserBreaksAsync(cancellationToken, id, dayOfWeek));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user breaks.");
                throw;
            }
        }

        public async Task<List<UserSchedule>> GetUserDaysOffAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUserDaysOffAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user breaks.");
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

        public async Task<UserDTO?> GetUserInfoByPhone(CancellationToken cancellationToken, string phone, UserStatus status = UserStatus.Active)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetUserInfoByPhone(cancellationToken, phone, status));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user ID.");
                throw;
            }
        }

        public async Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetByCompanyAndUserSlugAsync(cancellationToken, companySlug, userSlug) ?? throw new NotFoundException("Usuário não encontrado com o link."));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user by slugs: {@CompanySlug}/{@UserSlug}.", companySlug, userSlug);
                throw;
            }
        }

        public async Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetByIdAsync(cancellationToken, id) ?? throw new NotFoundException("Usuário não encontrado."));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user by ID.");
                throw;
            }
        }

        public async Task<List<CustomerDetailDTO>> GetUserCustomersAsync(CancellationToken cancellationToken, long userId)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await customerService.GetUserCustomersAsync(cancellationToken, userId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user by ID.");
                throw;
            }
        }

        public async Task<List<ServiceInformationDTO>> GetEnabledServicesAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetEnabledServicesAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await userRepository.GetServicesAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task UploadImageAsync(CancellationToken cancellationToken, long id, byte[]? photo)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.UploadImageAsync(cancellationToken, id, photo));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task CancelAllByDateAsync(CancellationToken cancellationToken, long id, DateTime date)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await appointmentService.CancelAllByDateAsync(cancellationToken, id, date));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
                {
                    var appointments = await appointmentService.GetNextAppointmentsAsync(cancellationToken, id);

                    // Estamos completando os agendamentos que já passaram do horário porem estamos fazendo um update em um metodo get, vamos ter que melhorar isso
                    foreach (var appointment in appointments.Where(w => w.Date <= LocalTime.Now && w.Status == AppointmentStatus.Scheduled).ToList())
                    {
                        var duration = appointment.Services.Select(w => w.Service).Sum(w => w.Duration);
                        if (appointment.Date.AddMinutes(duration) >= LocalTime.Now)
                            continue;

                        appointment.Status = AppointmentStatus.Completed;
                    }
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    return appointments;
                }, true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting next appointments.");
                throw;
            }
        }

        public async Task StopProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.StopProvidingServiceAsync(cancellationToken, id, serviceId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task StartProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.StartProvidingServiceAsync(cancellationToken, id, serviceId));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task ChangeDeviceToken(CancellationToken cancellationToken, long id, string deviceToken)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.ChangeDeviceToken(cancellationToken, id, deviceToken));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error adding service to user.");
                throw;
            }
        }

        public async Task<List<AppointmentDetailDTO>> GetAppointmentsAsyncV2(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetAppointmentsAsyncV2(cancellationToken, id, getUserAppointmentsDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting user appointments");
                throw;
            }
        }

        public async Task CancelAllScheduledAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await appointmentService.CancelAllScheduledAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error cancelling scheduled appointments.");
                throw;
            }
        }

        public async Task CancelAllUserScheduledByDateAsync(CancellationToken cancellationToken, long id, DateTime startDate, DateTime endDate)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await appointmentService.CancelAllUserScheduledByDateAsync(cancellationToken, id, startDate, endDate));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error cancelling scheduled appointments.");
                throw;
            }
        }

        public async Task CreateOwnerUserAsync(CancellationToken cancellationToken, CreateOwnerUserDTO createOwnerUserDto)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateOwnerUserAsync(cancellationToken, createOwnerUserDto));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating owner user.");
                throw;
            }
        }

        public async Task CreateEmployeeUserAsync(CancellationToken cancellationToken, CreateEmployeeUserDTO createEmployeeUser)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateEmployeeUserAsync(cancellationToken, createEmployeeUser));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating employee user.");
                throw;
            }
        }

        public async Task FinalizeEmployeeUserRegistrationAsync(CancellationToken cancellationToken, long id, FinalizeEmployeeUserDTO finalizeEmployeeUserDto)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.FinalizeEmployeeUserRegistrationAsync(cancellationToken, id, finalizeEmployeeUserDto));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error finalizing employee user.");
                throw;
            }
        }

        public async Task CreateBarbershopSchemeAsync(CancellationToken cancellationToken, CreateBarbershopSchemeDto createBarbershopSchemeDto)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateBarbershopSchemeAsync(cancellationToken, createBarbershopSchemeDto));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating owner user.");
                throw;
            }
        }

        public async Task<long> GetCompanyIdByIdAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await userRepository.GetCompanyIdByIdAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting companyId");
                throw;
            }
        }

        public async Task RemoveAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.RemoveAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error removing user.");
                throw;
            }
        }

        public async Task<int> GetUnreadNotificationsCountAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await userRepository.GetUnreadNotificationsCountAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting unread notifications count.");
                throw;
            }
        }

        public async Task ResetUnreadNotificationsAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.ResetUnreadNotificationsAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting unread notifications count.");
                throw;
            }
        }
    }
}