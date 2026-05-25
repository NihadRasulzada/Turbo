using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Infrastructure.EmailWorker;
using Turbo.Module.Identity.Infrastructure.Messaging;
using Turbo.Module.Identity.Infrastructure.Options;
using Turbo.Module.Identity.Infrastructure.Services;
using Turbo.Module.Identity.Persistence;
using Turbo.Module.Identity.Persistence.Context;
using Turbo.Module.Identity.Persistence.Middleware;

namespace Turbo.Module.Identity.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Options
        services.AddOptions<JwtOptions>()
            .Bind(config.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RabbitMqOptions>()
            .Bind(config.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(config.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // DbContexts
        services.AddDbContext<IdentityCommandContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("IdentityCommandDb")));

        services.AddDbContext<IdentityQueryContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("IdentityQueryDb")));

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        // MediatR + Validators (Infrastructure assembly-dən)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));



        // Background Workers
        services.AddHostedService<EmailConsumerWorker>();


        return services;
    }
}