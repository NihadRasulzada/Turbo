namespace Turbo.Shared.Application.Abstraction.Repository;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    DbSet<TEntity> Table { get; }
}