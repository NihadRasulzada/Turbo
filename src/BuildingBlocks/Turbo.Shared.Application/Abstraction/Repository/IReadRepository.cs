using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Turbo.Shared.Domain.Models;

namespace Turbo.Shared.Application.Abstraction.Repository;

public interface IReadRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(
        Guid id,
        bool enableTracking,
        CancellationToken cancellationToken,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
    );

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool enableTracking,
        CancellationToken cancellationToken,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
    );

    IQueryable<TEntity> GetAll(
        bool enableTracking,
        CancellationToken cancellationToken,
        bool ignoreQueryFilters = false,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    );

    Task<IEnumerable<TEntity>> GetAllAsync(
        bool enableTracking,
        CancellationToken cancellationToken,
        bool ignoreQueryFilters = false,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    );

    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        bool enableTracking,
        bool ignoreQueryFilters,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    );

    Task<int> CountAsync(
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = null
    );

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    );
}