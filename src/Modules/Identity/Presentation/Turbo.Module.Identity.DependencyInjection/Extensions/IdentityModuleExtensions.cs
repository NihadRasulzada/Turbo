using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Infrastructure.EmailWorker;
using Turbo.Module.Identity.Infrastructure.Messaging;
using Turbo.Module.Identity.Infrastructure.Options;
using Turbo.Module.Identity.Infrastructure.Services;
using Turbo.Module.Identity.Persistence.Contexts;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.Login;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.DependencyInjection.Extensions;

public static class IdentityModuleExtensions
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Options ───────────────────────────────────────────────────────────
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ── DbContexts ────────────────────────────────────────────────────────
        services.AddDbContext<CommandDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("CommandDb")));

        services.AddDbContext<QueryDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("QueryDbApp")));

        // ── DbContext interface aliases ────────────────────────────────────────
        services.AddScoped<IIdentityWriteDbContext>(sp => sp.GetRequiredService<CommandDbContext>());
        services.AddScoped<IIdentityReadDbContext>(sp => sp.GetRequiredService<QueryDbContext>());

        // ── Services ──────────────────────────────────────────────────────────
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        // ── Handlers ──────────────────────────────────────────────────────────
        services.AddScoped<
            ICommandHandler<RegisterUserRequest, AppConc.Response<RegisterUserResponse>>,
            RegisterUserHandler>();
        services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserValidator>();

        services.AddScoped<
            ICommandHandler<LoginRequest, AppConc.Response<LoginResponse>>,
            LoginHandler>();
        services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

        services.AddScoped<
            ICommandHandler<RefreshTokenRequest, AppConc.Response<RefreshTokenResponse>>,
            RefreshTokenHandler>();

        services.AddScoped<
            ICommandHandler<ChangePasswordRequest, AppConc.Response>,
            ChangePasswordHandler>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordValidator>();

        services.AddScoped<
            ICommandHandler<BlockUserRequest, AppConc.Response>,
            BlockUserHandler>();

        services.AddScoped<
            ICommandHandler<UnblockUserRequest, AppConc.Response>,
            UnblockUserHandler>();

        // ── Background Workers ────────────────────────────────────────────────
        services.AddHostedService<EmailConsumerWorker>();

        return services;
    }

    /// <summary>
    /// Identity modulu üçün pending migration-ları hər iki DB-yə tətbiq edir.
    /// QueryDb (admin) istifadə edilir — QueryDbApp (read-only) yox.
    /// </summary>
    public static async Task MigrateIdentityAsync(this IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<CommandDbContext>>();

        var commandConnStr = config.GetConnectionString("CommandDb")
            ?? throw new InvalidOperationException("ConnectionStrings:CommandDb tapılmadı.");
        var queryConnStr = config.GetConnectionString("QueryDb")
            ?? throw new InvalidOperationException("ConnectionStrings:QueryDb tapılmadı.");

        await using var commandCtx = new CommandDbContext(
            new DbContextOptionsBuilder<CommandDbContext>()
                .UseNpgsql(commandConnStr)
                .Options);
        logger.LogInformation("[Identity] CommandDb migration tətbiq edilir...");
        await commandCtx.Database.MigrateAsync();

        await using var queryCtx = new QueryDbContext(
            new DbContextOptionsBuilder<QueryDbContext>()
                .UseNpgsql(queryConnStr)
                .Options);
        logger.LogInformation("[Identity] QueryDb migration tətbiq edilir...");
        await queryCtx.Database.MigrateAsync();

        logger.LogInformation("[Identity] Migration tamamlandı.");
    }
}
