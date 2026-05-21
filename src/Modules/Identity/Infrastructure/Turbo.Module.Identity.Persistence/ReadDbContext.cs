using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Application.ReadModels;

namespace Turbo.Module.Identity.Persistence;

public class ReadDbContext(DbContextOptions<ReadDbContext> options)
    : DbContext(options), IReadDbContext
{
    public DbSet<UserReadModel> Users => Set<UserReadModel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserReadModel>().ToTable("UserReadModels").HasKey(u => u.Id);
    }
}
