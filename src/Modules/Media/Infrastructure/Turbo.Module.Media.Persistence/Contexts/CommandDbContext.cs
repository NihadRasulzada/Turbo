using Microsoft.EntityFrameworkCore;

namespace Turbo.Module.Media.Persistence.Contexts;

public class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : MediaDbContext(options) { }
