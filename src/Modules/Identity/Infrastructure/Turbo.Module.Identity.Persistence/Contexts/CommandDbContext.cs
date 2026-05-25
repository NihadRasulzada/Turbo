using Microsoft.EntityFrameworkCore;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Identity.Persistence.Contexts;

public sealed class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : IdentityDbContext(options), IIdentityWriteDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CommandDbContext).Assembly);
    }

    // ── IWriteDbContext explicit implementations ──────────────────────────────
    void IWriteDbContext.Attach<TEntity>(TEntity entity)
        => Attach(entity);

    void IWriteDbContext.AttachRange<TEntity>(IEnumerable<TEntity> entities)
        => AttachRange(entities);

    void IWriteDbContext.Add<TEntity>(TEntity entity)
        => Add(entity);

    void IWriteDbContext.AddRange<TEntity>(IEnumerable<TEntity> entities)
        => AddRange(entities);

    void IWriteDbContext.Remove<TEntity>(TEntity entity)
        => Remove(entity);
}
