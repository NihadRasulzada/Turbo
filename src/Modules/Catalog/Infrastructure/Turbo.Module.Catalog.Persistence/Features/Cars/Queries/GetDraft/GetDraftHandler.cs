using Turbo.Module.Catalog.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;

public sealed class GetDraftHandler(ICatalogReadDbContext db)
    : IQueryHandler<GetDraftRequest, AppConc.Response<GetDraftResponse>>
{
    public async Task<AppConc.Response<GetDraftResponse>> HandleAsync(
        GetDraftRequest query,
        CancellationToken ct = default)
    {
        var draft = await db.CarDrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.DraftId, ct);

        if (draft is null)
            return AppConc.Response<GetDraftResponse>.NotFound("Draft not found.");

        // Yalnız draftin sahibi öz draft-ını görə bilər.
        if (draft.SellerId != query.RequesterId)
            return AppConc.Response<GetDraftResponse>.Forbidden(
                "You do not have access to this draft.");

        return AppConc.Response<GetDraftResponse>.Success(
            new GetDraftResponse(draft.Id, draft.Status, draft.CurrentStep, 3));
    }
}