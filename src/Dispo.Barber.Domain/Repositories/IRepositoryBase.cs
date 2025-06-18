using System.Linq.Expressions;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.Repositories
{
    public interface IRepositoryBase<T> where T : EntityBase
    {
        Task AddAsync(CancellationToken cancellationToken, T entity);

        void Update(T entity);

        void UpdateRange(List<T> entities);

        void Delete(T entity);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<T?> GetAsync(CancellationToken cancellationToken, long id);

        Task<T?> GetAsNoTrackingAsync(CancellationToken cancellationToken, long id);

        Task<List<T>> GetAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression, string include = "");

        Task<List<T>> GetAsNoTrackingAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression, string include = "");

        Task<List<T>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken);

        Task<T> GetFirstAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression);

        Task<T> GetFirstAsNoTrackingAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression);

        Task<T> GetFirstAsync(CancellationToken cancellationToken, long id, string include = "");

        Task<T> GetFirstAsNoTrackingAsync(CancellationToken cancellationToken, long id, string include = "");

        Task<bool> ExistsAsync(CancellationToken cancellationToken, Expression<Func<T, bool>> expression);
    }
}