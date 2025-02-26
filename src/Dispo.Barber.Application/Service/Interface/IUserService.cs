using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IUserService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO);
        Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, List<long> services);
        Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);
        Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id);
        Task<List<UserSchedule>> GetUserBreaksAsync(CancellationToken cancellationToken, long id, DayOfWeek dayOfWeek);
        Task<List<UserSchedule>> GetUserDaysOffAsync(CancellationToken cancellationToken, long id);
        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO);
        Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO);
        Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO);
        Task<long> GetUserIdByPhone(CancellationToken cancellationToken, string phone);
        Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug);
        Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id);
        Task<List<UserDTO>> GetByCompanyId(CancellationToken cancellationToken, long companyId);
        Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id);
        Task UploadImageAsync(CancellationToken cancellationToken, long id, byte[]? photo);
        Task<bool> StopProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);
        Task<bool> StartProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);
        Task ChangeDeviceToken(CancellationToken cancellationToken, long id, string deviceToken);
        Task<List<AppointmentDetailDTO>> GetAppointmentsAsyncV2(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);
        Task CreateOwnerUserAsync(CancellationToken cancellationToken, CreateOwnerUserDTO createOwnerUserDto);
        Task CreateEmployeeUserAsync(CancellationToken cancellationToken, CreateEmployeeUserDTO createEmployeeUser);
        Task FinalizeEmployeeUserRegistrationAsync(CancellationToken cancellationToken, long id, FinalizeEmployeeUserDTO finalizeEmployeeUserDto);
        Task CreateBarbershopSchemeAsync(CancellationToken cancellationToken, CreateBarbershopSchemeDto createBarbershopSchemeDto);
    }
}
