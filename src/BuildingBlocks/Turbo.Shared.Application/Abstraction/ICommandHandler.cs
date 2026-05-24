using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Shared.Application.Abstraction;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : Response
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct = default);
}
