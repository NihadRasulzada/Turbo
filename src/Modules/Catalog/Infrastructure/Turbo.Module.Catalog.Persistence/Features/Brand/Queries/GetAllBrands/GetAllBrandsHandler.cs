using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;

public sealed class GetAllBrandsHandler(QueryDbContext db)
    : IQueryHandler<GetAllBrandsRequest, AppConc.Response<IReadOnlyList<BrandResponse>>>
{
    public async Task<AppConc.Response<IReadOnlyList<BrandResponse>>> HandleAsync(
        GetAllBrandsRequest query, CancellationToken ct = default)
    {
        var brands = await db.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandResponse(b.Id, b.Name))
            .ToListAsync(ct);

        return AppConc.Response<IReadOnlyList<BrandResponse>>.Success(brands);
    }
}
