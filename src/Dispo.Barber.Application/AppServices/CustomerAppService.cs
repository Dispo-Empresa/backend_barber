using Dispo.Barber.Application.AppServices.Interface;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppServices
{
    public class CustomerAppService(ILogger<CustomerAppService> logger, IUnitOfWork unitOfWork, ICustomerService service) : ICustomerAppService
    {
        public async Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetCustomersAsync(cancellationToken));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting customers.");
                throw;
            }
        }

        public async Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetCustomerAppointmentsAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting customers.");
                throw;
            }
        }

        public async Task<List<Customer>> GetForAppointment(CancellationToken cancellationToken, string search)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetForAppointment(cancellationToken, search));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting customers.");
                throw;
            }
        }

        public async Task<CustomerDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            try
            {
                return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () => await service.GetByIdAsync(cancellationToken, id));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error getting customers.");
                throw;
            }
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CustomerDTO customerDTO)
        {
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () => await service.CreateAsync(cancellationToken, customerDTO));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating customers.");
                throw;
            }
        }
    }
}
