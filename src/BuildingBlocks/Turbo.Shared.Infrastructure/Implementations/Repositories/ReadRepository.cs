using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Turbo.Shared.Application.Abstraction.Repository;
using Turbo.Shared.Application.Context;
using Turbo.Shared.Domain.Models;

namespace Turbo.Shared.Infrastructure.Implementations.Repositories;

public class ReadRepository<TEntity>(AppDbContext context)
    : Repository<TEntity>(context),
        IReadRepository<TEntity>
    where TEntity : BaseEntity
{
    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
    {
        return await Table.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = null
    )
    {
        return predicate == null
            ? await Table.CountAsync(cancellationToken)
            : await Table.CountAsync(predicate, cancellationToken);
    }

    public IQueryable<TEntity> GetAll(
        bool enableTracking,
        CancellationToken cancellationToken,
        bool ignoreQueryFilters = false,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
    {
        IQueryable<TEntity> query = Table;

        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();

        if (!enableTracking)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        if (include != null)
            query = include(query);

        if (orderBy != null)
            query = orderBy(query);

        return query;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        bool enableTracking,
        CancellationToken cancellationToken,
        bool ignoreQueryFilters = false,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
    {
        IQueryable<TEntity> query = GetAll(
            enableTracking,
            cancellationToken,
            ignoreQueryFilters,
            predicate,
            include,
            orderBy
        );
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool enableTracking,
        CancellationToken cancellationToken,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
    )
    {
        IQueryable<TEntity> query = Table;

        if (!enableTracking)
            query = query.AsNoTracking();

        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        bool enableTracking,
        CancellationToken cancellationToken,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
    )
    {
        IQueryable<TEntity> query = Table;

        if (!enableTracking)
            query = query.AsNoTracking();

        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        bool enableTracking,
        bool ignoreQueryFilters,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
    {
        IQueryable<TEntity> query = Table;

        if (!enableTracking)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        int totalCount = await query.CountAsync(cancellationToken);

        if (include != null)
            query = include(query);

        if (orderBy != null)
            query = orderBy(query);

        List<TEntity> items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }
}