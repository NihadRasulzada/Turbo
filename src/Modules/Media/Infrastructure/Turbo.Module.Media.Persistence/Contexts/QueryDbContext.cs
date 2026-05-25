using Microsoft.EntityFrameworkCore;

namespace Turbo.Module.Media.Persistence.Contexts;

public sealed class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : MediaDbContext(options), IMediaReadDbContext
{
    /// <summary>
    /// Query DB is read-only — writes must go through <see cref="CommandDbContext"/>.
    /// </summary>
    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException(
            "QueryDbContext is read-only. Use CommandDbContext to persist changes.");

    /// <inheritdoc cref="SaveChangesAsync"/>
    public override int SaveChanges() =>
        throw new InvalidOperationException(
            "QueryDbContext is read-only. Use CommandDbContext to persist changes.");
}