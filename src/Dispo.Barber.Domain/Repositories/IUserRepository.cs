using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<List<Appointment>> GetAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);

        Task<User> GetWithAppointmentsAsync(CancellationToken cancellationToken, long id);

        Task<List<UserSchedule>> GetValidDaysSchedulesAsync(CancellationToken cancellationToken, long id);
        Task<List<UserSchedule>> GetBreaksAsync(CancellationToken cancellationToken, long id, DayOfWeek dayOfWeek);
        Task<List<UserSchedule>> GetEnabledBreaksAsync(CancellationToken cancellationToken, long id, DayOfWeek dayOfWeek);
        Task<List<UserSchedule>> GetDaysOffAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetUserByBusinessAsync(long businessId);

        Task<UserDTO?> GetUserInfoByPhone(CancellationToken cancellationToken, string phone, UserStatus status = UserStatus.Active);

        Task<User> GetByPhoneWithBusinessUnitiesAsync(CancellationToken cancellationToken, string phone);

        Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug);

        Task<List<UserDTO>> GetByCompanyId(CancellationToken cancellationToken, long companyId);

        Task<UserDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id);

        Task<List<ServiceInformationDTO>> GetEnabledServicesAsync(CancellationToken cancellationToken, long id);

        Task<List<ServiceInformationDTO>> GetServicesAsync(CancellationToken cancellationToken, long id);

        Task<bool> StopProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);

        Task<bool> StartProvidingServiceAsync(CancellationToken cancellationToken, long id, long serviceId);

        Task<List<AppointmentDetailDTO>> GetAppointmentsAsyncV2(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);

        Task<User> GetByIdWithBusinessUnitiesAsync(CancellationToken cancellationToken, long id);

        Task<long> GetCompanyIdByIdAsync(CancellationToken cancellationToken, long id);

        Task<long> GetBusinessUnityIdByIdAsync(CancellationToken cancellationToken, long id);
    }
}
