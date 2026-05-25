using Turbo.Module.Catalog.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetAllModels;

public sealed class GetAllModelsHandler(ICatalogReadDbContext db)
    : IQueryHandler<GetAllModelsRequest, AppConc.Response<IReadOnlyList<GetAllModelsResponse>>>
{
    public async Task<AppConc.Response<IReadOnlyList<GetAllModelsResponse>>> HandleAsync(
        GetAllModelsRequest query, CancellationToken ct = default)
    {
        var q = db.Models.AsNoTracking();

        if (query.BrandId.HasValue)
            q = q.Where(m => m.BrandId == query.BrandId.Value);

        var models = await q
            .OrderBy(m => m.Name)
            .Select(m => new GetAllModelsResponse(m.Id, m.Name, m.BrandId))
            .ToListAsync(ct);

        return AppConc.Response<IReadOnlyList<GetAllModelsResponse>>.Success(models);
    }
}
