using Turbo.Shared.Application.Context;

namespace Turbo.Module.Media.Persistence.Contexts;

/// <summary>
/// Write context for the Media database.
/// Exposes only the generic mutation helpers from <see cref="IWriteDbContext"/>
/// — no entity-set properties — so consumers use <c>Set&lt;Media&gt;()</c>
/// for tracked bulk queries and the typed helpers for individual mutations.
/// </summary>
public interface IMediaWriteDbContext : IWriteDbContext { }
