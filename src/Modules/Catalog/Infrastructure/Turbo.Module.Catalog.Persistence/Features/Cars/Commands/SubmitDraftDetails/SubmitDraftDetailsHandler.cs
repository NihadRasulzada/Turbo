using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

public sealed class SubmitDraftDetailsHandler(CommandDbContext db)
    : ICommandHandler<SubmitDraftDetailsRequest, AppConc.Response<SubmitDraftDetailsResponse>>
{
    public async Task<AppConc.Response<SubmitDraftDetailsResponse>> HandleAsync(
        SubmitDraftDetailsRequest command,
        CancellationToken ct = default)
    {
        var draft = await db.CarDrafts.FindAsync([command.DraftId], ct);
        if (draft is null)
            return AppConc.Response<SubmitDraftDetailsResponse>.NotFound("Draft not found.");
        if (draft.Status == CarDraftStatus.Completed)
            return AppConc.Response<SubmitDraftDetailsResponse>.BadRequest("Draft is already completed.");
        if (draft.CurrentStep != 2)
            return AppConc.Response<SubmitDraftDetailsResponse>.BadRequest(
                draft.CurrentStep < 2
                    ? "Complete the images step first."
                    : "Details step has already been submitted.");

        var brand = await db.Brands.FindAsync([command.BrandId], ct);
        if (brand is null)
            return AppConc.Response<SubmitDraftDetailsResponse>.NotFound("Brand not found.");

        var model = await db.Models
            .FirstOrDefaultAsync(m => m.Id == command.ModelId && m.BrandId == command.BrandId, ct);
        if (model is null)
            return AppConc.Response<SubmitDraftDetailsResponse>.NotFound(
                "Model not found or does not belong to the specified brand.");

        draft.SetDetails(
            command.BrandId, command.ModelId, command.Year,
            command.FuelType, command.TransmissionType, command.Mileage);
        draft.AdvanceStep();
        await db.SaveChangesAsync(ct);

        return AppConc.Response<SubmitDraftDetailsResponse>.Success(
            new SubmitDraftDetailsResponse(command.DraftId, 2, 3, "pricing"));
    }
}
