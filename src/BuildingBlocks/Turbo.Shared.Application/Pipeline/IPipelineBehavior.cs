using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Shared.Application.Pipeline;

public delegate Task<TResponse> CommandHandlerDelegate<TResponse>() where TResponse : Response;

public interface IPipelineBehavior<TCommand, TResponse>
    where TCommand : Abstraction.ICommand<TResponse>
    where TResponse : Response
{
    Task<TResponse> HandleAsync(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken ct);
}