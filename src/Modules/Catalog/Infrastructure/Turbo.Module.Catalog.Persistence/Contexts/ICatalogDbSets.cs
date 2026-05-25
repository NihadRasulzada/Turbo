using Turbo.Module.Catalog.Domain.Entity;

namespace Turbo.Module.Catalog.Persistence.Contexts;

/// <summary>
/// Declares the entity sets exposed by the Catalog database.
/// Consumed by <see cref="ICatalogReadDbContext"/> (read handlers) and
/// implemented explicitly on <see cref="CatalogDbContext"/> via its EF <c>DbSet</c> properties.
/// <para>
/// Using <see cref="IQueryable{T}"/> instead of <c>DbSet&lt;T&gt;</c> keeps the interface
/// free of EF Core mutation surface (<c>Add</c>, <c>Remove</c>, …).
/// Command handlers that need to read a related entity must inject
/// <see cref="ICatalogReadDbContext"/> alongside <see cref="ICatalogWriteDbContext"/>.
/// </para>
/// </summary>
public interface ICatalogDbSets
{
    IQueryable<Car> Cars { get; }
    IQueryable<CarDraft> CarDrafts { get; }
    IQueryable<Brand> Brands { get; }
    IQueryable<Model> Models { get; }
}