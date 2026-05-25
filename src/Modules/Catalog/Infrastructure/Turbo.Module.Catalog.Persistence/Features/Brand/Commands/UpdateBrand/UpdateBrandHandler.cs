using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using DomainBrand = Turbo.Module.Catalog.Domain.Entity.Brand;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed class UpdateBrandHandler(ICatalogWriteDbContext writeDb)
    : ICommandHandler<UpdateBrandRequest, AppConc.Response<UpdateBrandResponse>>
{
    public async Task<AppConc.Response<UpdateBrandResponse>> HandleAsync(
        UpdateBrandRequest command, CancellationToken ct = default)
    {
        var brand = await writeDb.Set<DomainBrand>()
            .FirstOrDefaultAsync(b => b.Id == command.Id, ct);
        if (brand is null)
            return AppConc.Response<UpdateBrandResponse>.NotFound("Brand not found.");

        brand.UpdateName(command.Name);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response<UpdateBrandResponse>.Success(new UpdateBrandResponse(brand.Id, brand.Name));
    }
}
