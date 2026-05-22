using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;

public sealed class DeleteBrandHandler(CommandDbContext db)
    : ICommandHandler<DeleteBrandRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        DeleteBrandRequest command, CancellationToken ct = default)
    {
        var brand = await db.Brands.FindAsync([command.Id], ct);
        if (brand is null)
            return AppConc.Response.NotFound("Brand not found.");

        var hasModels = await db.Models.AnyAsync(m => m.BrandId == command.Id, ct);
        if (hasModels)
            return AppConc.Response.Conflict("Cannot delete brand with existing models.");

        db.Brands.Remove(brand);
        await db.SaveChangesAsync(ct);
        return AppConc.Response.NoContent();
    }
}
