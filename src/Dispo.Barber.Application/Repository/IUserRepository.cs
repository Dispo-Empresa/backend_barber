using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<List<Appointment>> GetAppointmentsAsync(CancellationToken cancellationToken, long id, GetUserAppointmentsDTO getUserAppointmentsDTO);

        Task<User> GetWithAppointmentsAsync(CancellationToken cancellationToken, long id);

        Task<List<UserSchedule>> GetValidDaysSchedulesAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetUserByBusinessAsync(long businessId);

        Task<long> GetIdByPhone(CancellationToken cancellationToken, string phone);

        Task<User> GetByPhoneWithBusinessUnitiesAsync(CancellationToken cancellationToken, string phone);

        Task<User?> GetByCompanyAndUserSlugAsync(CancellationToken cancellationToken, string companySlug, string userSlug);

        Task<List<UserDTO>> GetByCompanyId(CancellationToken cancellationToken, long companyId);
    }
}
