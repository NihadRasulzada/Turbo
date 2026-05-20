using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Persistence.Contexts;

public class IdentityDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
   
}
