namespace Turbo.Shared.Application.Abstraction.Repository;

public interface IWriteRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    Task HardDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task HardDeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
}