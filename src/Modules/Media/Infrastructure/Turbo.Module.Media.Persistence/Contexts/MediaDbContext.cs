using Microsoft.EntityFrameworkCore;
using Turbo.Module.Media.Domain.Entity;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Media.Persistence.Contexts;

public abstract class MediaDbContext(DbContextOptions options) : AppDbContext(options)
{
    public required DbSet<CarImage> CarImages { get; set; }
    public required DbSet<CarDraftImage> CarDraftImages { get; set; }
}
