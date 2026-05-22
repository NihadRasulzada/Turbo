using Microsoft.Extensions.DependencyInjection;
using Turbo.Shared.Application.Abstraction;

namespace Turbo.Shared.Infrastructure.Implementations;

public sealed class QueryDispatcher(IServiceProvider sp) : IQueryDispatcher
{
    public Task<TResponse> DispatchAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default
    )
        where TQuery : IQuery<TResponse>
    {
        var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        return handler.HandleAsync(query, ct);
    }
}