using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IBusinessUnityAppService
    {
        Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id);
    }
}
