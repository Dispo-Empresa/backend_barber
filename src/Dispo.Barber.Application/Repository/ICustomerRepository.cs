using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Repository
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetCustomersForAppointment(CancellationToken cancellationToken, string search);
    }
}
