using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftDetails;

public sealed class SubmitDraftDetailsHandler(
    CommandDbContext db,
    IValidator<SubmitDraftDetailsRequest> validator)
    : ICommandHandler<SubmitDraftDetailsRequest, AppConc.Response<DraftStepResponse>>
{
    public async Task<AppConc.Response<DraftStepResponse>> HandleAsync(
        SubmitDraftDetailsRequest command,
        CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return AppConc.Response<DraftStepResponse>.ValidationError(
                validation.Errors.Select(e =>
                    new CustomValidationError(e.PropertyName, e.ErrorMessage)));

        var draft = await db.CarDrafts.FindAsync([command.DraftId], ct);
        if (draft is null)
            return AppConc.Response<DraftStepResponse>.NotFound("Draft not found.");
        if (draft.Status == CarDraftStatus.Completed)
            return AppConc.Response<DraftStepResponse>.BadRequest("Draft is already completed.");
        if (draft.CurrentStep != 2)
            return AppConc.Response<DraftStepResponse>.BadRequest(
                draft.CurrentStep < 2
                    ? "Complete the images step first."
                    : "Details step has already been submitted.");

        var brand = await db.Brands.FindAsync([command.BrandId], ct);
        if (brand is null)
            return AppConc.Response<DraftStepResponse>.NotFound("Brand not found.");

        var model = await db.Models
            .FirstOrDefaultAsync(m => m.Id == command.ModelId && m.BrandId == command.BrandId, ct);
        if (model is null)
            return AppConc.Response<DraftStepResponse>.NotFound(
                "Model not found or does not belong to the specified brand.");

        draft.SetDetails(
            command.BrandId, command.ModelId, command.Year,
            command.FuelType, command.TransmissionType, command.Mileage);
        draft.AdvanceStep();
        await db.SaveChangesAsync(ct);

        return AppConc.Response<DraftStepResponse>.Success(
            new DraftStepResponse(command.DraftId, 2, 3, "pricing", false));
    }
}
