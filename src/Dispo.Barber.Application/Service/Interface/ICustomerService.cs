using Dispo.Barber.Domain.DTO;
using Dispo.Barber.Domain.DTO.Customer;
namespace Dispo.Barber.Application.Service.Interface
{
    public interface ICustomerService
    {
        Task<CustomerDTO> CreateAsync(CustomerDTO customerDTO);
        Task<CustomerDTO> GetByPhoneAsync(string phone);
    }
}
