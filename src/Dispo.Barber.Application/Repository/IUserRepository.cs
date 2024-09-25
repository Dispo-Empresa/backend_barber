using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<List<Appointment>> GetAppointmentsAsync(CancellationToken cancellationToken, long id);
        Task<User> GetWithAppointmentsAsync(CancellationToken cancellationToken, long id);
    }
}
