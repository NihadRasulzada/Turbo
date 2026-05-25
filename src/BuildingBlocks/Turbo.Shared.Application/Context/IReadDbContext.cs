namespace Turbo.Shared.Application.Context;

/// <summary>
/// Marker interface for read-only (query) database contexts.
/// Implementations must not expose <c>SaveChanges</c> or <c>SaveChangesAsync</c>.
/// </summary>
public interface IReadDbContext { }