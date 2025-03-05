using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Repositories
{
    public interface IServiceUserRepository : IRepositoryBase<ServiceUser>
    {
        Task<List<User>> GetUsersByServiceId(List<long> serviceUserIds);
        Task<List<Service>> GetServicesByUserId(long serviceUserId);
    }
}
