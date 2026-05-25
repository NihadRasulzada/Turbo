using Turbo.Module.Catalog.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;

public sealed class GetModelByIdHandler(ICatalogReadDbContext db)
    : IQueryHandler<GetModelByIdRequest, AppConc.Response<GetModelByIdResponse>>
{
    public async Task<AppConc.Response<GetModelByIdResponse>> HandleAsync(
        GetModelByIdRequest query, CancellationToken ct = default)
    {
        var model = await db.Models
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == query.Id, ct);

        return model is null
            ? AppConc.Response<GetModelByIdResponse>.NotFound("Model not found.")
            : AppConc.Response<GetModelByIdResponse>.Success(
                new GetModelByIdResponse(model.Id, model.Name, model.BrandId));
    }
}
