using MassTransit;
using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Contracts.IntegrationEvents;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftImages;

public sealed class SubmitDraftImagesHandler(
    CommandDbContext db,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<SubmitDraftImagesRequest, AppConc.Response<SubmitDraftImagesResponse>>
{
    public async Task<AppConc.Response<SubmitDraftImagesResponse>> HandleAsync(
        SubmitDraftImagesRequest command,
        CancellationToken ct = default)
    {
        if (command.Images.Count == 0)
            return AppConc.Response<SubmitDraftImagesResponse>.BadRequest("At least one image is required.");

        var draft = await db.CarDrafts.FindAsync([command.DraftId], ct);
        if (draft is null)
            return AppConc.Response<SubmitDraftImagesResponse>.NotFound("Draft not found.");
        if (draft.Status == CarDraftStatus.Completed)
            return AppConc.Response<SubmitDraftImagesResponse>.BadRequest("Draft is already completed.");
        if (draft.CurrentStep != 1)
            return AppConc.Response<SubmitDraftImagesResponse>.BadRequest("Images step has already been submitted.");

        draft.AdvanceStep();
        await db.SaveChangesAsync(ct);

        await publishEndpoint.Publish(
            new DraftImagesUploadedIntegrationEvent(command.DraftId, command.Images), ct);

        return AppConc.Response<SubmitDraftImagesResponse>.Success(
            new SubmitDraftImagesResponse(command.DraftId, 1, 3, "details"));
    }
}
