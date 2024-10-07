using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IBusinessUnityAppService
    {
        Task<List<User>> GetUsersAsync(long id);
    }
}
