using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IUserAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateUserDTO createUserDTO);

        Task AddServiceToUserAsync(CancellationToken cancellationToken, long id, List<long> addServiceToUserDTO);

        Task<List<Appointment>> GetUserAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);

        Task<List<UserSchedule>> GetUserSchedulesAsync(CancellationToken cancellationToken, long id);

        Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateUserDTO updateUserDTO);

        Task ChangeStatusAsync(CancellationToken cancellationToken, long id, ChangeStatusDTO changeStatusDTO);

        Task ChangePasswordAsync(CancellationToken cancellationToken, long id, ChangePasswordDTO changePasswordDTO);

        Task<long> GetUserIdByPhone(CancellationToken cancellationToken, string phone);

        Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug);

        Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id);

        Task<List<CustomerDetailDTO>> GetUserCustomersAsync(CancellationToken cancellationToken, long userId);

        Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id);

        Task UploadImageAsync(CancellationToken cancellationToken, long id, byte[]? photo);

        Task CancelAllByDateAsync(CancellationToken cancellationToken, long id, DateTime date);

        Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long id);

        Task StopProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);

        Task StartProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);
        Task ChangeDeviceToken(CancellationToken cancellationToken, long id, string deviceToken);
    }
}