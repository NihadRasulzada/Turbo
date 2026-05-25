using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Shared.Application.Context;

namespace Turbo.Module.Identity.Application.Context;

public class IdentityDbContext(DbContextOptions options) : AppDbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}