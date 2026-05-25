using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Persistence.Contexts;

/// <summary>
/// Declares the entity sets exposed by the Identity database.
/// Consumed by <see cref="IIdentityReadDbContext"/> (read handlers) and
/// implemented explicitly on <see cref="IdentityDbContext"/> via its EF DbSet properties.
/// Using <see cref="IQueryable{T}"/> instead of DbSet keeps the interface
/// free of EF Core mutation surface.
/// </summary>
public interface IIdentityDbSets
{
    IQueryable<User> Users { get; }
    IQueryable<RefreshToken> RefreshTokens { get; }
}
