using MediaEntity = Turbo.Module.Media.Domain.Entity.Media;

namespace Turbo.Module.Media.Persistence.Contexts;

/// <summary>
/// Declares the entity sets exposed by the Media database.
/// Consumed by <see cref="IMediaReadDbContext"/> (read side) and
/// implemented explicitly on <see cref="MediaDbContext"/> via its EF <c>DbSet</c> property.
/// </summary>
public interface IMediaDbSets
{
    IQueryable<MediaEntity> Medias { get; }
}