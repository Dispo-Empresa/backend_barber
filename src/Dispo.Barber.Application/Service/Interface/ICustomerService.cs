using Dispo.Barber.Domain.DTO;
namespace Dispo.Barber.Application.Service.Interface
{
    public interface ICustomerService
    {
        Task<long> CreateAsync(CustomerDTO customerDTO);
        Task<CustomerDTO> GetByPhoneAsync(string phone);
    }
}
