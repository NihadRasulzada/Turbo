using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Catalog.Persistence.Contexts;

public abstract class CatalogDbContext(DbContextOptions options) : AppDbContext(options)
{
    public required DbSet<Car> Cars { get; set; }
    public required DbSet<CarDraft> CarDrafts { get; set; }
    public required DbSet<Brand> Brands { get; set; }
    public required DbSet<Model> Models { get; set; }
}
