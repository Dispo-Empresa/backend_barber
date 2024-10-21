using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Utils;


namespace Dispo.Barber.Application.Service
{
    public class CustomerService(IUnitOfWork unitOfWork, IMapper mapper) : ICustomerService
    {
        public async Task<CustomerDTO> CreateAsync(CustomerDTO customerDTO)
        {
            var cancellationTokenSource = new CancellationTokenRegistration();
            customerDTO.Phone = PhoneNumberUtils.FormatPhoneNumber(customerDTO.Phone);
            var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
            var customerInformation = await GetByPhoneAsync(customerDTO.Phone);
            if (customerInformation == null)
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
                {
                    var customer = mapper.Map<Customer>(customerDTO);
                    await customerRepository.AddAsync(cancellationTokenSource.Token, customer);
                    await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);


                });

                return await GetByPhoneAsync(customerDTO.Phone);
            }
            else
            {
                return customerInformation;
            }
        }

        public async Task<CustomerDTO> GetByPhoneAsync(string phone)
        {
            phone = PhoneNumberUtils.FormatPhoneNumber(phone);
            var cancellationTokenSource = new CancellationTokenSource();
            var customer = await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
                return await customerRepository.GetCustomerByPhoneAsync(phone);
            });

            return mapper.Map<CustomerDTO>(customer);
        }
    }
}
