using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;

public sealed class CreateDraftHandler(ICatalogWriteDbContext writeDb)
    : ICommandHandler<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>
{
    public async Task<AppConc.Response<CreateDraftResponse>> HandleAsync(
        CreateDraftRequest command,
        CancellationToken ct = default)
    {
        var draft = new CarDraft();
        writeDb.Add(draft);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response<CreateDraftResponse>.Created(new CreateDraftResponse(draft.Id));
    }
}