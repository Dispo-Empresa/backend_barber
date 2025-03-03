using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppServices.Interface
{
    public interface IBusinessUnityAppService
    {
        Task<List<User>> GetUsersAsync(CancellationToken cancellationToken, long id);

        Task<List<User>> GetPendingUsersAsync(CancellationToken cancellationToken, long id);

        Task<List<CustomerDetailDTO>> GetBusinessUnityCustomersAsync(CancellationToken cancellationToken, long businessUnityId);
    }
}
