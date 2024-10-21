using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        Task<List<Customer>> GetCustomersForAppointment(CancellationToken cancellationToken, string search);

        Task<Customer> GetCustomerByPhoneAsync(string phone);

        Task<long> GetCustomerIdByPhoneAsync(string phone);
    }
}
