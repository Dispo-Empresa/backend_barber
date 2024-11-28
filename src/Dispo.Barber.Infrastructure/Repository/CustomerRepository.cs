using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.DTO.Service;
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

        public async Task<List<CustomerDetailDTO>> GetUserCustomersAsync(CancellationToken cancellationToken, long userId)
        {
            return await context.Appointments.Include(i => i.AcceptedUser)
                                      .Include(i => i.Customer)
                                      .Where(w => w.AcceptedUserId == userId)
                                      .Select(s => new CustomerDetailDTO
                                      {
                                          Id = s.Customer.Id,
                                          Name = s.Customer.Name,
                                          LastAppointment = s.Date,
                                      })
                                      .ToListAsync();
        }

        public async Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Appointments
                          .Where(w => w.CustomerId == id)
                          .Select(s => new AppointmentDetailDTO
                          {
                              Id = s.Id,
                              Data = s.Date,
                              Services = s.Services.Select(s => s.Service).Select(s => new ServiceDetailDTO
                              {
                                  Id = s.Id,
                                  Description = s.Description,
                              }).ToList()
                          })
                          .ToListAsync();
        }

        public async Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            return await context.Appointments.Select(s => new CustomerDetailDTO
            {
                Id = s.Customer.Id,
                Name = s.Customer.Name,
                LastAppointment = s.Date,
            }).ToListAsync();
        }

        public async Task<CustomerDetailDTO?> GetByIdAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Customers.Include(i => i.Appointments)
                .Where(w => w.Id == id)
                .Select(s => new CustomerDetailDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Frequency = s.Appointments.Count,
                    LastAppointment = s.Appointments != null && s.Appointments.Count != 0 ? s.Appointments.OrderByDescending(o => o.Id).First().Date : null,
                }).FirstOrDefaultAsync();
        }
    }
}
