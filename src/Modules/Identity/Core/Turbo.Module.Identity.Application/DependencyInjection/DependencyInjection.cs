using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Turbo.Module.Identity.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}