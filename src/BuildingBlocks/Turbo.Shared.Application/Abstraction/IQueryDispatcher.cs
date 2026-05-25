namespace Turbo.Shared.Application.Abstraction;

public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResponse>;
}