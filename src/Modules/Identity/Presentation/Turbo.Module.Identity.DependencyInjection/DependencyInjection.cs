using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Application.Validators;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Infrastructure.Messaging;
using Turbo.Module.Identity.Infrastructure.Services;
using Turbo.Module.Identity.Persistence;
using Turbo.Module.Identity.Persistence.Services;

namespace Turbo.Module.Identity.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        // DbContext
        services.AddDbContext<IdentityDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("IdentityDb")));

        // ASP.NET Core Identity
        services.AddIdentity<AppUser, IdentityRole<Guid>>(opts =>
        {
            opts.Password.RequiredLength = 8;
            opts.Password.RequireUppercase = true;
            opts.Password.RequireLowercase = true;
            opts.Password.RequireDigit = true;
            opts.Password.RequireNonAlphanumeric = true;

            opts.User.RequireUniqueEmail = true;

            opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            opts.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Services
        services.AddScoped<TokenService>();
        services.AddScoped<IUserService, UserService>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();

        return services;
    }
}
