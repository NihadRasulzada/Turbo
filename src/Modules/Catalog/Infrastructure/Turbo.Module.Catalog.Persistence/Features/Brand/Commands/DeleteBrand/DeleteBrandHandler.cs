using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainBrand = Turbo.Module.Catalog.Domain.Entity.Brand;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;

public sealed class DeleteBrandHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<DeleteBrandRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        DeleteBrandRequest command, CancellationToken ct = default)
    {
        var brand = await readDb.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == command.Id, ct);
        if (brand is null)
            return AppConc.Response.NotFound("Brand not found.");

        var hasModels = await readDb.Models.AnyAsync(m => m.BrandId == command.Id, ct);
        if (hasModels)
            return AppConc.Response.Conflict("Cannot delete brand with existing models.");

        // EF Core attaches the untracked entity in Deleted state automatically.
        writeDb.Remove(brand);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response.NoContent();
    }
}
