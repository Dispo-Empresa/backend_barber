using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Repositories
{
    public interface ICustomerRepository : IRepositoryBase<Customer>
    {
        Task<List<Customer>> GetCustomersForAppointment(CancellationToken cancellationToken, string search);

        Task<Customer> GetCustomerByPhoneAsync(string phone);

        Task<long> GetCustomerIdByPhoneAsync(string phone);

        Task<List<CustomerDetailDTO>> GetUserCustomersAsync(CancellationToken cancellationToken, long userId);

        Task<List<CustomerDetailDTO>> GetBusinessUnityCustomersAsync(CancellationToken cancellationToken, long businessUnityId);

        Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id);

        Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken);

        Task<CustomerDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id);
    }
}
