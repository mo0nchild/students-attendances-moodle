
using Microsoft.EntityFrameworkCore.Storage;

namespace Attendances.Domain.Core.Repositories;

public interface IBaseRepository : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
    
    Task<IDbContextTransaction> BeginTransactionAsync();
    IDbContextTransaction BeginTransaction();
}