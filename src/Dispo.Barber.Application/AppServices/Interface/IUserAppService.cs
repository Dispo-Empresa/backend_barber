using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface IUserAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO);
        Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, List<long> addServiceToUserDTO);
        Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);
        Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id);
        Task<List<UserSchedule>> GetUserBreaksAsync(CancellationToken cancellationToken, long id, DayOfWeek dayOfWeek);
        Task<List<UserSchedule>> GetUserDaysOffAsync(CancellationToken cancellationToken, long id);
        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO);
        Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO);
        Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO);
        Task<UserDTO?> GetUserInfoByPhone(CancellationToken cancellationToken, string phone, UserStatus status = UserStatus.Active);
        Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug);
        Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<List<CustomerDetailDTO>> GetUserCustomersAsync(CancellationToken cancellationToken, long userId);
        Task<List<ServiceInformationDTO>> GetEnabledServicesAsync(CancellationToken cancellationToken, long id);
        Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id);
        Task UploadImageAsync(CancellationToken cancellationToken, long id, byte[]? photo);
        Task CancelAllByDateAsync(CancellationToken cancellationToken, long id, DateTime date);
        Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long id);
        Task StopProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);
        Task StartProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);
        Task ChangeDeviceToken(CancellationToken cancellationToken, long id, string deviceToken);
        Task<List<AppointmentDetailDTO>> GetAppointmentsAsyncV2(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);
        Task CancelAllScheduledAsync(CancellationToken cancellationToken, long id);
        Task CancelAllUserScheduledByDateAsync(CancellationToken cancellationToken, long id, DateTime startDate, DateTime endDate);
        Task CreateOwnerUserAsync(CancellationToken cancellationToken, CreateOwnerUserDTO createOwnerUserDto);
        Task CreateEmployeeUserAsync(CancellationToken cancellationToken, CreateEmployeeUserDTO createOwnerUserDto);
        Task FinalizeEmployeeUserRegistrationAsync(CancellationToken cancellationToken, long id, FinalizeEmployeeUserDTO createOwnerUserDto);
        Task CreateBarbershopSchemeAsync(CancellationToken cancellationToken, CreateBarbershopSchemeDto createBarbershopSchemeDto);
        Task<long> GetCompanyIdByIdAsync(CancellationToken cancellationToken, long id);
        Task RemoveAsync(CancellationToken cancellationToken, long id);
    }
}