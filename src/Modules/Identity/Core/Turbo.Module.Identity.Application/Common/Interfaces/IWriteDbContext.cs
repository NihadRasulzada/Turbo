using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Application.Common.Interfaces;

public interface IWriteDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
