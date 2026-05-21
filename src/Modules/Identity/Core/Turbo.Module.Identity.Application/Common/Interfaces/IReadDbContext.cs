using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.ReadModels;

namespace Turbo.Module.Identity.Application.Common.Interfaces;

public interface IReadDbContext
{
    DbSet<UserReadModel> Users { get; }
}
