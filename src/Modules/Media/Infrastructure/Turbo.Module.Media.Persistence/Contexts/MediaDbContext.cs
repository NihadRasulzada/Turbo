using Microsoft.EntityFrameworkCore;
using Turbo.Module.Media.Domain.Entity;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Media.Persistence.Contexts;

public class MediaDbContext(DbContextOptions<MediaDbContext> options) : AppDbContext(options)
{
    public required DbSet<CarImage> CarImages { get; set; }
}
