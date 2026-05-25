using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainBrand = Turbo.Module.Catalog.Domain.Entity.Brand;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed class UpdateBrandHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<UpdateBrandRequest, AppConc.Response<UpdateBrandResponse>>
{
    public async Task<AppConc.Response<UpdateBrandResponse>> HandleAsync(
        UpdateBrandRequest command, CancellationToken ct = default)
    {
        // Load untracked from read DB; attach to write DB before mutating
        // so the change tracker captures only the delta.
        var brand = await readDb.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == command.Id, ct);
        if (brand is null)
            return AppConc.Response<UpdateBrandResponse>.NotFound("Brand not found.");

        writeDb.Attach(brand);          // Unchanged state — snapshot recorded
        brand.UpdateName(command.Name); // EF detects the change
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response<UpdateBrandResponse>.Success(new UpdateBrandResponse(brand.Id, brand.Name));
    }
}
