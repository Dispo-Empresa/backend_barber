using Dispo.Barber.Application.Repository;
using Dispo.Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

public class UnitOfWork(ApplicationContext context, IServiceProvider serviceProvider) : IUnitOfWork
{
    private bool _disposed;
    private string _errorMessage = string.Empty;
    private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

    private IDbContextTransaction? transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (transaction == null) 
        {
            transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (transaction is null)
        {
            throw new NullReferenceException("No open transaction to commit.");
        }

        try
        {
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (transaction is null)
        {
            return;
        }

        await transaction.RollbackAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        if (context is null)
        {
            throw new NullReferenceException("No open context to save.");
        }

        return await context.SaveChangesAsync(cancellationToken);
    }

    private async Task DisposeTransactionAsync()
    {
        if (transaction is null)
        {
            return;
        }

        await transaction.DisposeAsync();
    }

    public T GetRepository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (T)_repositories[typeof(T)];
        }

        var repository = serviceProvider.GetRequiredService<T>();
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            context?.Dispose();
            transaction?.Dispose();
        }

        _disposed = true;
    }

    public async Task ExecuteUnderTransactionAsync(CancellationToken cancellationToken, Func<Task> action)
    {
        try
        {
            await BeginTransactionAsync(cancellationToken);

            await action();

            await CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task<T> QueryUnderTransactionAsync<T>(CancellationToken cancellationToken, Func<Task<T>> action, bool commit = false)
    {
        try
        {
            await BeginTransactionAsync(cancellationToken);

            var result = await action();
            if (commit)
            {
                await CommitAsync(cancellationToken);
            }
            return result;
        }
        catch
        {
            if (commit)
            {
                await RollbackAsync(cancellationToken);
            }
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }
}
