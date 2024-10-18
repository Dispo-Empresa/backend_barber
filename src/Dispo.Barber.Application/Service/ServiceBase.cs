using System.Linq.Expressions;
using System.Threading;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service
{
    public class ServiceBase<T> : IServiceBase<T> where T : EntityBase
    {
        private readonly IRepositoryBase<T> repository;

        public ServiceBase(IRepositoryBase<T> repository)
        {
            this.repository = repository;
        }

        public async Task AddAsync(T entity)
        {
            await repository.AddAsync(entity);
        }

        public void Update(T entity)
        {
            repository.Update(entity);
        }

        public void Delete(T entity)
        {
            repository.Delete(entity);
        }

        public async Task<T?> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetAsync(cancellationToken, id);
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await repository.GetAsync(expression);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }
    }
}
