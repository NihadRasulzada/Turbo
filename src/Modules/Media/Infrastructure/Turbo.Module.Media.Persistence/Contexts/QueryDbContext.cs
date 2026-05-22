using Microsoft.EntityFrameworkCore;

namespace Turbo.Module.Media.Persistence.Contexts;

public class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : MediaDbContext(options) { }
