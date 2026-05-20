using Turbo.Shared.Application.Abstraction.Repository;
using Turbo.Shared.Application.Context;
using Turbo.Shared.Domain.Models;

namespace Turbo.Shared.Infrastructure.Implementations.Repositories;

public class WriteRepository<TEntity>(AppDbContext context, IReadRepository<TEntity> readRepository)
    : Repository<TEntity>(context),
        IWriteRepository<TEntity>
    where TEntity : BaseEntity
{
    public AppDbContext Context => context;

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await Table.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken
    )
    {
        await Table.AddRangeAsync(entities, cancellationToken);
    }

    public virtual async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await readRepository
            .GetByIdAsync(id, true, cancellationToken: cancellationToken)
            .ContinueWith(
                task =>
                {
                    if (task.Result is not null)
                    {
                        Table.Remove(task.Result);
                    }
                },
                cancellationToken
            );
    }

    public virtual Task HardDeleteRangeAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken
    )
    {
        var entities = new List<TEntity>();
        return Task.WhenAll(
                ids.Select(id =>
                    readRepository
                        .GetByIdAsync(id, true, cancellationToken: cancellationToken)
                        .ContinueWith(
                            task =>
                            {
                                if (task.Result is not null)
                                {
                                    entities.Add(task.Result);
                                }
                            },
                            cancellationToken
                        )
                )
            )
            .ContinueWith(_ => Table.RemoveRange(entities), cancellationToken);
    }

    public void Update(TEntity entity)
    {
        Table.Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        Table.UpdateRange(entities);
    }
}