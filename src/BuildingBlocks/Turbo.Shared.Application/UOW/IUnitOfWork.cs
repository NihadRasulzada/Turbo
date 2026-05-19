using Microsoft.EntityFrameworkCore.Storage;
using Turbo.Shared.Application.Context;

namespace Turbo.Shared.Application.UOW;

public interface IUnitOfWork<TDbContext> : IAsyncDisposable
    where TDbContext : AppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}
