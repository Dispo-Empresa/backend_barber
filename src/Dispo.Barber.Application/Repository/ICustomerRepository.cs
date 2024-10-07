using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        Task<Customer> GetCustomerByPhoneAsync(string phone);
    }
}
