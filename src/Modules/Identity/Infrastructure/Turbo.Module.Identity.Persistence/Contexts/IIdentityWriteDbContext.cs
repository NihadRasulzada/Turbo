using Turbo.Shared.Application.Context;

namespace Turbo.Module.Identity.Persistence.Contexts;

/// <summary>
/// Write context for the Identity database.
/// Exposes only the generic mutation helpers from <see cref="IWriteDbContext"/>.
/// Command handlers must inject <see cref="IIdentityReadDbContext"/> when
/// they need to read an entity before mutating it.
/// </summary>
public interface IIdentityWriteDbContext : IWriteDbContext { }
