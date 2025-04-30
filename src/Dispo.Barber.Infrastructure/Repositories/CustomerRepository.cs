using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repositories
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

        public async Task<long> GetCustomerIdByPhoneAsync(string phone, CancellationToken cancellationToken)
        {
            return await context.Customers
                    .Where(w => w.Phone == phone)
                    .Select(w => w.Id)
                    .FirstOrDefaultAsync(cancellationToken);

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
            return await context.Appointments.Include(i => i.Customer)
                                             .Where(a => a.AcceptedUserId == userId)
                                             .GroupBy(a => new { a.Customer.Id, a.Customer.Name, a.Customer.Phone })
                                             .Select(g => new CustomerDetailDTO
                                             {
                                                 Id = g.Key.Id,
                                                 Name = g.Key.Name,
                                                 Phone = g.Key.Phone,
                                                 LastAppointment = g.Max(a => a.Date),
                                                 Frequency = g.Count()
                                             })
                                             .ToListAsync(cancellationToken);
        }

        public async Task<List<CustomerDetailDTO>> GetBusinessUnityCustomersAsync(CancellationToken cancellationToken, long businessUnityId)
        {
            return await context.Appointments.Include(i => i.Customer)
                                             .Where(a => a.BusinessUnityId == businessUnityId)
                                             .GroupBy(a => new { a.Customer.Id, a.Customer.Name, a.Customer.Phone })
                                             .Select(g => new CustomerDetailDTO
                                             {
                                                 Id = g.Key.Id,
                                                 Name = g.Key.Name,
                                                 Phone = g.Key.Phone,
                                                 LastAppointment = g.Max(a => a.Date),
                                                 Frequency = g.Count()
                                             })
                                             .ToListAsync(cancellationToken);
        }

        public async Task<List<AppointmentDetailDTO>> GetCustomerAppointmentsAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Appointments
                          .Where(w => w.CustomerId == id)
                          .Select(s => new AppointmentDetailDTO
                          {
                              Id = s.Id,
                              Date = s.Date,
                              Status = s.Status,
                              Services = s.Services.Select(s => s.Service).Select(s => new ServiceDetailDTO
                              {
                                  Id = s.Id,
                                  Description = s.Description,
                              }).ToList()
                          })
                          .OrderByDescending(x => x.Date)
                          .ToListAsync();
        }

        public async Task<List<CustomerDetailDTO>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            return await context.Appointments.Select(s => new CustomerDetailDTO
            {
                Id = s.Customer.Id,
                Name = s.Customer.Name,
                Phone = s.Customer.Phone,
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

        public async Task<bool> HasMultipleAppointmentsAsync(CancellationToken cancellation, long id)
        {
            return await context.Customers.Include(i => i.Appointments)
               .AnyAsync(w => w.Id == id && w.Appointments.Count(w => w.Status == AppointmentStatus.Scheduled) > 3);
        }
    }
}
