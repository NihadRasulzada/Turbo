using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Application.Settings;
using Turbo.Module.Media.Infrastructure.Services;
using Turbo.Module.Media.Infrastructure.Settings;
using Turbo.Module.Media.Persistence.BackgroundServices;
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
        // ── Options ──────────────────────────────────────────────────────────
        services.Configure<MinioSettings>(opts =>
        {
            opts.Endpoint = configuration["MinIO:Endpoint"] ?? string.Empty;
            opts.AccessKey = configuration["MinIO:AccessKey"] ?? string.Empty;
            opts.SecretKey = configuration["MinIO:SecretKey"] ?? string.Empty;
            opts.BucketName = configuration["MinIO:BucketName"] ?? string.Empty;
            opts.UseSSL = string.Equals(
                configuration["MinIO:UseSSL"],
                "true",
                StringComparison.OrdinalIgnoreCase
            );
        });

        services.Configure<ImageResizeSettings>(opts =>
        {
            if (int.TryParse(configuration["ImageResize:MaxWidth"], out var w)) opts.MaxWidth = w;
            if (int.TryParse(configuration["ImageResize:MaxHeight"], out var h)) opts.MaxHeight = h;
            if (int.TryParse(configuration["ImageResize:BatchSize"], out var b)) opts.BatchSize = b;
            if (int.TryParse(configuration["ImageResize:PollingIntervalSeconds"], out var p))
                opts.PollingIntervalSeconds = p;
        });

        // ── MinIO ─────────────────────────────────────────────────────────────
        services.AddSingleton<IMinioClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            return new MinioClient()
                .WithEndpoint(settings.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        });

        services.AddScoped<IMinioService, MinioService>();

        // ── Image resize ──────────────────────────────────────────────────────
        services.AddScoped<IImageResizeService, ImageResizeService>();
        services.AddHostedService<ImageResizeBackgroundService>();

        // ── DbContexts ────────────────────────────────────────────────────────
        services.AddDbContext<CommandDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("CommandDb"))
        );

        services.AddDbContext<QueryDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("QueryDb"))
        );

        return services;
    }

    public static void AddMediaConsumers(this IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<DraftImagesUploadedConsumer>();
        cfg.AddConsumer<CarListingPublishedConsumer>();
    }
}