using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface IServiceUserRepository : IRepositoryBase <ServiceUser>
    {
        Task<List<User>> GetUsersByServiceId(List<long> serviceUserIds);
        Task<List<Dispo.Barber.Domain.Entities.Service>> GetServicesByUserId(long serviceUserId);
    }
}
