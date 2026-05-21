using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Infrastructure.Messaging;
using Turbo.Module.Identity.Infrastructure.Services;
using Turbo.Module.Identity.Persistence;

namespace Turbo.Module.Identity.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // Write DB
        services.AddDbContext<WriteDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("WriteDb")));

        services.AddDbContext<ReadDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("ReadDb")));

        services.AddScoped<IWriteDbContext>(sp =>
            sp.GetRequiredService<WriteDbContext>());
        services.AddScoped<IReadDbContext>(sp =>
            sp.GetRequiredService<ReadDbContext>());

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
}
