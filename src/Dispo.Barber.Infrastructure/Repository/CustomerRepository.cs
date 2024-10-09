﻿using Dispo.Barber.Application.Repository;
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

        public async Task<List<Customer>> GetCustomersForAppointment(string search)
        {
            if (search.Any(char.IsDigit))
            {
                return await context.Customers.Where(w => w.Phone.Contains(search))
                                              .ToListAsync();
            }
         
            return await context.Customers.Where(w => w.Name.Contains(search))
                                          .ToListAsync();
        }

    }
}