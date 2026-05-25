using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Context;

namespace Turbo.Module.Identity.Persistence.Context;

public sealed class IdentityCommandContext(
    DbContextOptions<IdentityCommandContext> options
) : IdentityDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityCommandContext).Assembly);
    }
}
