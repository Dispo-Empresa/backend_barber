using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        private readonly ApplicationContext context;
        public CustomerRepository(ApplicationContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phone)
        {
            return await context.Customers
                   .FirstOrDefaultAsync(w => w.Phone == phone);
        }

        public async Task<long> GetCustomerIdByPhoneAsync(string phone)
        {
            return await context.Customers
                    .Where(w => w.Phone == phone)
                    .Select(w => w.Id)
                    .FirstOrDefaultAsync();

        }

        public async Task<List<Customer>> GetCustomersForAppointment(CancellationToken cancellationToken, string search)
        {
            if (search.Any(char.IsDigit))
            {
                return await context.Customers.Where(w => w.Phone.Contains(search))
                                              .ToListAsync(cancellationToken);
            }
         
            return await context.Customers.Where(w => w.Name.Contains(search))
                                          .ToListAsync(cancellationToken);
        }

    }
}
