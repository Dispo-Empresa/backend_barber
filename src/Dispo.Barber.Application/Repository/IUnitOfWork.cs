namespace Dispo.Barber.Application.Repository
{
    public interface IUnitOfWork
    {
        Task ExecuteUnderTransactionAsync(CancellationToken cancellationToken, Func<Task> action);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        T GetRepository<T>() where T : class;
        Task<T> QueryUnderTransactionAsync<T>(CancellationToken cancellationToken, Func<Task<T>> action, bool commit = false, bool ignoreRollback = false);
    }
}
