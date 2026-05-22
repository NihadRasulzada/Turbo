using Microsoft.EntityFrameworkCore;

namespace Turbo.Shared.Application.Context;

public abstract class AppDbContext(DbContextOptions options) : DbContext(options) { }
