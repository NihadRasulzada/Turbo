using Turbo.Shared.Application.Context;

namespace Turbo.Module.Identity.Persistence.Contexts;

/// <summary>
/// Read-only view of the Identity database.
/// Inject this in query handlers and in command handlers that need to
/// read an entity without loading it into the write-side change tracker.
/// </summary>
public interface IIdentityReadDbContext : IReadDbContext, IIdentityDbSets { }
