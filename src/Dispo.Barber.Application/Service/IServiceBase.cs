using System.Linq.Expressions;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service
{
    public interface IServiceBase<T> where T : EntityBase
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetAsync(long id);
        Task<List<T>> GetAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAllAsync();
    }
}
