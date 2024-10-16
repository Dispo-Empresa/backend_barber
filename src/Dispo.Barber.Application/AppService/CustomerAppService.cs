using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService
{
    public class CustomerAppService(IUnitOfWork unitOfWork) : ICustomerAppService
    {
        public async Task<List<Customer>> GetForAppointment(CancellationToken cancellationToken, string search)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
                return await customerRepository.GetCustomersForAppointment(cancellationToken, search);
            });
        }
    }
}
