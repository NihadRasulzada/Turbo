using Microsoft.EntityFrameworkCore;
using Turbo.Shared.Application.Context;
using MediaEntity = Turbo.Module.Media.Domain.Entity.Media;

namespace Turbo.Module.Media.Persistence.Contexts;

public abstract class MediaDbContext(DbContextOptions options) : AppDbContext(options)
{
    public required DbSet<MediaEntity> Medias { get; set; }
}
