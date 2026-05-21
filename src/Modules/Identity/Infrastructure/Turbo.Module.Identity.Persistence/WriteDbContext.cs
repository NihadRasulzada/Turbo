using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Persistence;

public class WriteDbContext(DbContextOptions<WriteDbContext> options)
    : DbContext(options), IWriteDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);
    }
}
