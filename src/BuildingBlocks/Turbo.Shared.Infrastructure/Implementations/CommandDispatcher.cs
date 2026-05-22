using Microsoft.Extensions.DependencyInjection;
using Turbo.Shared.Application.Abstraction;

namespace Turbo.Shared.Infrastructure.Implementations;

public sealed class CommandDispatcher(IServiceProvider sp) : ICommandDispatcher
{
    public Task<TResponse> DispatchAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default
    )
        where TCommand : ICommand<TResponse>
    {
        var handler = sp.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        return handler.HandleAsync(command, ct);
    }
}