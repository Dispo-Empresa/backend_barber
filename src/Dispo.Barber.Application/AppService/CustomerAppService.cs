using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService
{
    public class CustomerAppService(IUnitOfWork unitOfWork) : ICustomerAppService
    {
        public async Task<List<Customer>> GetForAppointment(string search)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var customerRepository = unitOfWork.GetRepository<ICustomerRepository>();
                return await customerRepository.GetCustomersForAppointment(search);
            });
        }
    }
}
