using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppServices.Interfaces
{
    public interface ICustomerAppService
    {
        Task<List<Customer>> GetForAppointment(CancellationToken cancellationToken, string search);

        Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id);

        Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken);

        Task<CustomerDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id);

        Task CreateAsync(CancellationToken cancellationToken, CustomerDTO customerDTO);
    }
}
