using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.CreateDraft;

public sealed class CreateDraftHandler(CommandDbContext db)
    : ICommandHandler<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>
{
    public async Task<AppConc.Response<CreateDraftResponse>> HandleAsync(
        CreateDraftRequest command,
        CancellationToken ct = default)
    {
        var draft = new CarDraft();
        await db.CarDrafts.AddAsync(draft, ct);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<CreateDraftResponse>.Created(new CreateDraftResponse(draft.Id));
    }
}