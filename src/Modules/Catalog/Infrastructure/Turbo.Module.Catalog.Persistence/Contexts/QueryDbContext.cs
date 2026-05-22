using Microsoft.EntityFrameworkCore;

namespace Turbo.Module.Catalog.Persistence.Contexts;

public class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : CatalogDbContext(options) { }