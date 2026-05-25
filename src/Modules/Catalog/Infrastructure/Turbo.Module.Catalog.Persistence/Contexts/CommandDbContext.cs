using Microsoft.EntityFrameworkCore;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Catalog.Persistence.Contexts;

public sealed class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : CatalogDbContext(options), ICatalogWriteDbContext
{
    // ── IWriteDbContext explicit implementations ─────────────────────────
    IQueryable<TEntity> IWriteDbContext.Set<TEntity>()
        => Set<TEntity>();

    void IWriteDbContext.Add<TEntity>(TEntity entity)
        => Add(entity);

    void IWriteDbContext.AddRange<TEntity>(IEnumerable<TEntity> entities)
        => AddRange(entities);

    void IWriteDbContext.Remove<TEntity>(TEntity entity)
        => Remove(entity);
}
