using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;

public sealed class GetModelByIdHandler(QueryDbContext db)
    : IQueryHandler<GetModelByIdRequest, AppConc.Response<ModelResponse>>
{
    public async Task<AppConc.Response<ModelResponse>> HandleAsync(
        GetModelByIdRequest query, CancellationToken ct = default)
    {
        var model = await db.Models
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == query.Id, ct);

        return model is null
            ? AppConc.Response<ModelResponse>.NotFound("Model not found.")
            : AppConc.Response<ModelResponse>.Success(new ModelResponse(model.Id, model.Name, model.BrandId));
    }
}
