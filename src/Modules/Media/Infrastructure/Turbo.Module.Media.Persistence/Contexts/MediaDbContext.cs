using Microsoft.EntityFrameworkCore;
using Turbo.Shared.Application.Context;
using MediaEntity = Turbo.Module.Media.Domain.Entity.Media;

namespace Turbo.Module.Media.Persistence.Contexts;

/// <summary>
/// Base EF Core context shared by <see cref="CommandDbContext"/> (write)
/// and <see cref="QueryDbContext"/> (read).
/// Implements <see cref="IMediaDbSets"/> so that both concrete contexts
/// satisfy the read interface without repeating property declarations.
/// The explicit interface member exposes <c>DbSet&lt;Media&gt;</c> as
/// <see cref="IQueryable{T}"/>, stripping EF mutation surface from the interface.
/// </summary>
public abstract class MediaDbContext(DbContextOptions options) : AppDbContext(options), IMediaDbSets
{
    public DbSet<MediaEntity> Medias { get; set; } = null!;

    // ── IMediaDbSets explicit implementation ────────────────────────────
    IQueryable<MediaEntity> IMediaDbSets.Medias => Medias;
}