using Microsoft.EntityFrameworkCore;
using Turbo.Shared.Application.Abstraction.Repository;
using Turbo.Shared.Application.Context;
using Turbo.Shared.Domain.Models;

namespace Turbo.Shared.Infrastructure.Implementations.Repositories;

public class Repository<TEntity>(AppDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    public DbSet<TEntity> Table => context.Set<TEntity>();
}