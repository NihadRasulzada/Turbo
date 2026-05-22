using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Infrastructure.Services;
using Turbo.Module.Media.Infrastructure.Settings;
using Turbo.Module.Media.Persistence.Consumers;
using Turbo.Module.Media.Persistence.Contexts;

namespace Turbo.Module.Media.DependencyInjection.Extensions;

public static class MediaModuleExtensions
{
    public static IServiceCollection AddMediaModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("MinIO");

        services.Configure<MinioSettings>(opts =>
        {
            opts.Endpoint = section["Endpoint"] ?? string.Empty;
            opts.AccessKey = section["AccessKey"] ?? string.Empty;
            opts.SecretKey = section["SecretKey"] ?? string.Empty;
            opts.BucketName = section["BucketName"] ?? string.Empty;
            opts.UseSSL = string.Equals(section["UseSSL"], "true", StringComparison.OrdinalIgnoreCase);
        });

        services.AddMinio(cfg => cfg
            .WithEndpoint(section["Endpoint"] ?? string.Empty)
            .WithCredentials(section["AccessKey"] ?? string.Empty, section["SecretKey"] ?? string.Empty)
            .WithSSL(string.Equals(section["UseSSL"], "true", StringComparison.OrdinalIgnoreCase))
            .Build()
        );

        services.AddScoped<IMinioService, MinioService>();

        services.AddDbContext<MediaDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("MediaDb"))
        );

        return services;
    }

    // Called from Program.cs inside AddMassTransit to register consumers modularly
    public static void AddMediaConsumers(this IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<CarCreatedEventConsumer>();
    }
}
