using Dispo.Barber.Domain.DTO.BusinessUnity;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IBusinessUnityService
    {
        Task<long> CreateAsync(CancellationToken cancellationToken, CreateBusinessUnityDTO createBusinessUnityDTO);
        Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id);
    }
}
