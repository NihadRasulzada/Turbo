using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainBrand = Turbo.Module.Catalog.Domain.Entity.Brand;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;

public sealed class CreateBrandHandler(ICatalogWriteDbContext writeDb)
    : ICommandHandler<CreateBrandRequest, AppConc.Response<CreateBrandResponse>>
{
    public async Task<AppConc.Response<CreateBrandResponse>> HandleAsync(
        CreateBrandRequest command, CancellationToken ct = default)
    {
        var brand = new DomainBrand(command.Name);
        writeDb.Add(brand);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response<CreateBrandResponse>.Created(new CreateBrandResponse(brand.Id, brand.Name));
    }
}
