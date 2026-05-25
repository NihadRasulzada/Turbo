using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Identity.Persistence.Contexts;

/// <summary>
/// Base EF Core context shared by <see cref="CommandDbContext"/> (write)
/// and <see cref="QueryDbContext"/> (read).
/// Implements <see cref="IIdentityDbSets"/> so that both concrete contexts
/// satisfy the read interface without repeating property declarations.
/// </summary>
public abstract class IdentityDbContext(DbContextOptions options)
    : AppDbContext(options), IIdentityDbSets
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    // ── IIdentityDbSets explicit implementations ─────────────────────────────
    IQueryable<User> IIdentityDbSets.Users => Users;
    IQueryable<RefreshToken> IIdentityDbSets.RefreshTokens => RefreshTokens;
}
