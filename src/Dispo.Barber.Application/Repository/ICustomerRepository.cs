using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        Task<Customer> GetCustomerByPhoneAsync(string phone);
        Task<List<Customer>> GetCustomersForAppointment(string search);
        Task<long> GetCustomerIdByPhoneAsync(string phone);
    }
}
