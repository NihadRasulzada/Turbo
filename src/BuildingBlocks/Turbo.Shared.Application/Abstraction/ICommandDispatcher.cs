namespace Turbo.Shared.Application.Abstraction;

public interface ICommandDispatcher
{
    Task<TResponse> DispatchAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default
    )
        where TCommand : ICommand<TResponse>;
}