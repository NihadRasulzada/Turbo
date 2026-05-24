using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetBrandById;

public sealed class GetBrandByIdHandler(QueryDbContext db)
    : IQueryHandler<GetBrandByIdRequest, AppConc.Response<GetBrandByIdResponse>>
{
    public async Task<AppConc.Response<GetBrandByIdResponse>> HandleAsync(
        GetBrandByIdRequest query, CancellationToken ct = default)
    {
        var brand = await db.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == query.Id, ct);

        return brand is null
            ? AppConc.Response<GetBrandByIdResponse>.NotFound("Brand not found.")
            : AppConc.Response<GetBrandByIdResponse>.Success(new GetBrandByIdResponse(brand.Id, brand.Name));
    }
}
