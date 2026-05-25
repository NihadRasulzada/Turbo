using Turbo.Shared.Application.Context;

namespace Turbo.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Write context for the Catalog database.
/// Exposes only the generic mutation helpers from <see cref="IWriteDbContext"/>
/// — no entity-set properties — so command handlers are forced to
/// inject <see cref="ICatalogReadDbContext"/> when they need to read
/// a related entity before mutating.
/// </summary>
public interface ICatalogWriteDbContext : IWriteDbContext { }
