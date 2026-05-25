using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using DomainBrand = Turbo.Module.Catalog.Domain.Entity.Brand;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;

public sealed class DeleteBrandHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<DeleteBrandRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        DeleteBrandRequest command, CancellationToken ct = default)
    {
        var brand = await writeDb.Set<DomainBrand>()
            .FirstOrDefaultAsync(b => b.Id == command.Id, ct);
        if (brand is null)
            return AppConc.Response.NotFound("Brand not found.");

        var hasModels = await readDb.Models.AnyAsync(m => m.BrandId == command.Id, ct);
        if (hasModels)
            return AppConc.Response.Conflict("Cannot delete brand with existing models.");

        writeDb.Remove(brand);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response.NoContent();
    }
}
