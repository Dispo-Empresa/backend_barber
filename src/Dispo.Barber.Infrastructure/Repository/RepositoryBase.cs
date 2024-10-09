using System.Linq.Expressions;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Infrastructure.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
    {
        private readonly ApplicationContext context;

        public RepositoryBase(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(T entity)
        {
            await context.Set<T>()
                         .AddAsync(entity);
        }

        public void Update(T entity)
        {
            context.Update(entity);
        }

        public void Delete(T entity)
        {
            context.Remove(entity);
        }

        public async Task<T?> GetAsync(long id)
        {
            return await context.Set<T>()
                                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>()
                                .Where(expression)
                                .ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>()
                                .ToListAsync();
        }
    }
}
