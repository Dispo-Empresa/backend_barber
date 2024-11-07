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

        public async Task AddAsync(CancellationToken cancellationToken, T entity)
        {
            await context.Set<T>()
                         .AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            context.Update(entity);
        }

        public void Delete(T entity)
        {
            context.Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (context is null)
            {
                throw new NullReferenceException("No open context to save.");
            }

            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await context.Set<T>().AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<T>> GetAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>()
                                .Where(expression)
                                .ToListAsync(cancellationToken);
        }

        public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await context.Set<T>()
                                .ToListAsync(cancellationToken);
        }

        public async Task<T> GetFirstAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>()
                                .FirstOrDefaultAsync(expression, cancellationToken);
        }
    }
}
