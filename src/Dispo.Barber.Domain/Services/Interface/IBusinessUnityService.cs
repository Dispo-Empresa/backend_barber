using Dispo.Barber.Domain.DTOs.BusinessUnity;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IBusinessUnityService
    {
        Task<long> CreateAsync(CancellationToken cancellationToken, CreateBusinessUnityDTO createBusinessUnityDTO);
        Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id);
    }
}
