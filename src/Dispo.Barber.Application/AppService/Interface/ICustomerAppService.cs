using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface ICustomerAppService
    {
        Task<List<Customer>> GetForAppointment(CancellationToken cancellationToken, string search);

        Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id);

        Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken);
    }
}
