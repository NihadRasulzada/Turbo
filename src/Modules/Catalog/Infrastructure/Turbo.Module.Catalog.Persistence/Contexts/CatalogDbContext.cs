using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Base EF Core context shared by <see cref="CommandDbContext"/> (write)
/// and <see cref="QueryDbContext"/> (read).
/// Implements <see cref="ICatalogDbSets"/> so that both concrete contexts
/// satisfy the read interface without repeating property declarations.
/// The explicit interface members expose <c>DbSet&lt;T&gt;</c> as
/// <see cref="IQueryable{T}"/>, stripping EF mutation surface from the interface.
/// </summary>
public abstract class CatalogDbContext(DbContextOptions options)
    : AppDbContext(options), ICatalogDbSets
{
    public DbSet<Car> Cars { get; set; } = null!;
    public DbSet<CarDraft> CarDrafts { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Model> Models { get; set; } = null!;

    // ── ICatalogDbSets explicit implementations ─────────────────────────
    IQueryable<Car> ICatalogDbSets.Cars => Cars;
    IQueryable<CarDraft> ICatalogDbSets.CarDrafts => CarDrafts;
    IQueryable<Brand> ICatalogDbSets.Brands => Brands;
    IQueryable<Model> ICatalogDbSets.Models => Models;
}
