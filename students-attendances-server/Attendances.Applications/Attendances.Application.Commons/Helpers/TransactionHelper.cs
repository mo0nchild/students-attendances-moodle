using System.Collections.Concurrent;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Commons.Helpers;

public class InnerTransactionProcessor
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _transactionLocks;
    public InnerTransactionProcessor(ConcurrentDictionary<Guid, SemaphoreSlim> transactionLocks, 
        ILogger<InnerTransactionProcessor> logger)
    {
        _transactionLocks = transactionLocks;
        Logger = logger;
    }
    private ILogger<InnerTransactionProcessor> Logger { get; }

    public class TransactionResult(SemaphoreSlim semaphore, Guid lockUuid,
        ConcurrentDictionary<Guid, SemaphoreSlim> locks) : IDisposable
    {
        public void Dispose()
        {
            semaphore.Release();
            locks.TryRemove(lockUuid, out _);
        }
    }

    public async Task<TransactionResult> BeginInnerTransaction(Guid lockUuid, CancellationToken cancellationToken = default)
    {
        var semaphore = _transactionLocks.GetOrAdd(lockUuid, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        return new TransactionResult(semaphore, lockUuid, _transactionLocks);
    }
}
public static class TransactionHelper
{
    public static Task<IServiceCollection> AddInnerTransactionServices(this IServiceCollection serviceCollection, 
        string transactionName)
    {
        serviceCollection.AddKeyedSingleton<ConcurrentDictionary<Guid, SemaphoreSlim>>(transactionName);
        serviceCollection.AddKeyedTransient<InnerTransactionProcessor>(transactionName, (provider, _) =>
        {
            var concurrentDictionary = provider
                .GetRequiredKeyedService<ConcurrentDictionary<Guid, SemaphoreSlim>>(transactionName);
            var logger = provider.GetRequiredService<ILogger<InnerTransactionProcessor>>();
            return new InnerTransactionProcessor(concurrentDictionary, logger);
        });
        return Task.FromResult(serviceCollection);
    }
}

