using Microsoft.EntityFrameworkCore;

namespace Turbo.Module.Catalog.Persistence.Contexts;

public class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : CatalogDbContext(options) { }