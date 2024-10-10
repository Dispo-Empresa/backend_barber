using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Utils;
using System.Reflection.Metadata;


namespace Dispo.Barber.Application.Service
{
    public class CustomerService(IUnitOfWork unitOfWork, IMapper mapper) : ICustomerService
    {
        public async Task<long> CreateAsync(CustomerDTO customerDTO)
        {
            var cancellationTokenSource = new CancellationTokenRegistration();
            customerDTO.Phone = PhoneNumberUtils.FormatPhoneNumber(customerDTO.Phone);
            var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
            var idCustomer = await customerRepository.GetCustomerIdByPhoneAsync(customerDTO.Phone);
            if (idCustomer == 0)
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
                {
                    var customer = mapper.Map<Customer>(customerDTO);
                    await customerRepository.AddAsync(customer);
                    await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);


                });

                return await customerRepository.GetCustomerIdByPhoneAsync(customerDTO.Phone);
            }
            else
            {
                return idCustomer;
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
