using Turbo.Shared.Application.Context;

namespace Turbo.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Read-only view of the Catalog database.
/// Inject this in query handlers and in command handlers that need to
/// read a related entity without loading it into the write-side change tracker.
/// </summary>
public interface ICatalogReadDbContext : IReadDbContext, ICatalogDbSets { }
