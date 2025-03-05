using AutoMapper;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;


namespace Dispo.Barber.Domain.Services
{
    public class CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ICustomerRepository repository) : ICustomerService
    {
        public async Task<CustomerDTO> CreateAsync(CustomerDTO customerDTO)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                customerDTO.Phone = StringUtils.FormatPhoneNumber(customerDTO.Phone);
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
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao criar o cliente.", ex);
            }
        }

        public async Task<CustomerDTO> GetByPhoneAsync(string phone)
        {
            try
            {
                phone = StringUtils.FormatPhoneNumber(phone);
                var cancellationTokenSource = new CancellationTokenSource();
                var customer = await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
                {
                    var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
                    return await customerRepository.GetCustomerByPhoneAsync(phone);
                });

                return mapper.Map<CustomerDTO>(customer);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao buscar o cliente pelo telefone.", ex);
            }
        }

        public async Task<List<Customer>> GetForAppointment(CancellationToken cancellationToken, string search)
        {
            try
            {
                return await repository.GetCustomersForAppointment(cancellationToken, search);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao buscar clientes para o agendamento.", ex);
            }
        }

        public async Task<List<CustomerDetailDTO>> GetUserCustomersAsync(CancellationToken cancellationToken, long userId)
        {
            return await repository.GetUserCustomersAsync(cancellationToken, userId);
        }

        public async Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetCustomerAppointmentsAsync(cancellationToken, id);
        }

        public async Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            var customers = await repository.GetCustomersAsync(cancellationToken);
            var groupedCustomers = customers.GroupBy(g => g.Id);
            var customerDetails = new List<CustomerDetailDTO>();
            foreach (var customer in groupedCustomers)
            {
                customerDetails.Add(new CustomerDetailDTO
                {
                    Id = customer.First().Id,
                    Name = customer.First().Name,
                    Phone = customer.First().Phone,
                    LastAppointment = customer.OrderByDescending(o => o.LastAppointment).First().LastAppointment,
                    Frequency = customer.Count()
                });
            }
            return [.. customerDetails.OrderBy(o => o.Name)];
        }

        public async Task<CustomerDetailDTO> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetByIdAsync(cancellationToken, id) ?? throw new NotFoundException("Cliente não encontrado.");
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CustomerDTO customerDTO)
        {
            var customer = mapper.Map<Customer>(customerDTO);
            await repository.AddAsync(cancellationToken, customer);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
