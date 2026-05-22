using Microsoft.EntityFrameworkCore;

namespace Turbo.Shared.Application.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }

    protected AppDbContext() { }
}