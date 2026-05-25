namespace Turbo.Shared.Application.Context;

/// <summary>
/// Marker interface for write (command) database contexts.
/// Intentionally exposes <b>no</b> queryable surface (<c>Set&lt;T&gt;</c> is absent)
/// so that a command handler that tries to read through this interface will
/// not compile.  All reads must go through <see cref="IReadDbContext"/>.
/// <para>
/// Load-to-modify pattern:
/// <code>
/// var entity = await readDb.Entities.AsNoTracking().FirstOrDefaultAsync(...);
/// writeDb.Attach(entity);   // tell the write-side change tracker about it
/// entity.DoSomething();     // mutate via domain method
/// await writeDb.SaveChangesAsync(ct);
/// </code>
/// </para>
/// </summary>
public interface IWriteDbContext
{
    // ── Persistence ─────────────────────────────────────────────────────────
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // ── Attach (load-to-modify) ──────────────────────────────────────────────
    /// <summary>
    /// Attaches an entity that was loaded untracked (e.g. via <c>AsNoTracking()</c>
    /// from <see cref="IReadDbContext"/>) to the write-side change tracker.
    /// The entity enters <c>Unchanged</c> state; subsequent property mutations are
    /// detected automatically before <see cref="SaveChangesAsync"/>.
    /// </summary>
    void Attach<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>Attaches multiple untracked entities to the write-side change tracker.</summary>
    void AttachRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    // ── Add ─────────────────────────────────────────────────────────────────
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    // ── Remove ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Marks the entity for deletion.  If the entity is not yet tracked it is
    /// first attached in <c>Deleted</c> state, so loading it via
    /// <see cref="IReadDbContext"/> before calling this method is sufficient.
    /// </summary>
    void Remove<TEntity>(TEntity entity) where TEntity : class;
}