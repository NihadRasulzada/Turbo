namespace Turbo.Shared.Application.Context;

/// <summary>
/// Marker interface for write (command) database contexts.
/// Exposes generic mutation helpers and a tracked <see cref="Set{TEntity}"/>
/// so callers never need a module-specific entity-set property.
/// <para>
/// For load-to-modify scenarios use <c>Set&lt;T&gt;().FirstOrDefaultAsync(...)</c>
/// — the underlying <c>DbSet&lt;T&gt;</c> feeds the change tracker identically to
/// <c>DbContext.FindAsync</c> for entities not already loaded in the current scope.
/// </para>
/// </summary>
public interface IWriteDbContext
{
    // ── Persistence ────────────────────────────────────────────────────────
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // ── Tracked reads (load-to-modify pattern) ─────────────────────────────
    /// <summary>
    /// Returns a tracked <see cref="IQueryable{T}"/> backed by the write-side
    /// change tracker. Use for bulk load-to-modify queries.
    /// </summary>
    IQueryable<TEntity> Set<TEntity>() where TEntity : class;

    // ── Mutations ──────────────────────────────────────────────────────────
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}
