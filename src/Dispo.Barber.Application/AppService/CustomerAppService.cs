using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Dispo.Barber.Application.AppService
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
    }
}
