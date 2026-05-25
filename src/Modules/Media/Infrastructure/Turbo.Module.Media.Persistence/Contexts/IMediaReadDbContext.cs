using Turbo.Shared.Application.Context;

namespace Turbo.Module.Media.Persistence.Contexts;

/// <summary>
/// Read-only view of the Media database.
/// Inject this in query handlers — <c>SaveChangesAsync</c> is not available.
/// </summary>
public interface IMediaReadDbContext : IReadDbContext, IMediaDbSets { }
