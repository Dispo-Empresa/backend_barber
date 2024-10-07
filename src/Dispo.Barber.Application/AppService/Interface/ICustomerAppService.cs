using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface ICustomerAppService
    {
        Task<List<Customer>> GetForAppointment(string search);
    }
}
