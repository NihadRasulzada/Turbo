using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Turbo.Module.Catalog.Persistence.Contexts;

namespace Turbo.Module.Catalog.Persistence.DesignTime;

/// <summary>
/// EF CLI migrations üçün istifadə olunur.
/// Runtime-da QueryDbApp (read-only turbo_reader) istifadə edilir;
/// migration apply zamanı isə admin hüquqlu QueryDb connection string lazımdır.
/// </summary>
public sealed class QueryDbContextDesignTimeFactory : IDesignTimeDbContextFactory<QueryDbContext>
{
    public QueryDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = config.GetConnectionString("QueryDb")
            ?? throw new InvalidOperationException(
                "Connection string 'QueryDb' tapılmadı. " +
                "appsettings.Development.json faylını yoxlayın.");

        var optionsBuilder = new DbContextOptionsBuilder<QueryDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new QueryDbContext(optionsBuilder.Options);
    }
}
