using FluentValidation;
using MassTransit;
using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject;
using Turbo.Shared.Contracts.IntegrationEvents;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftPricing;

public sealed class SubmitDraftPricingHandler(
    CommandDbContext db,
    IValidator<SubmitDraftPricingRequest> validator,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<SubmitDraftPricingRequest, AppConc.Response<DraftStepResponse>>
{
    public async Task<AppConc.Response<DraftStepResponse>> HandleAsync(
        SubmitDraftPricingRequest command,
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
        if (draft.CurrentStep != 3)
            return AppConc.Response<DraftStepResponse>.BadRequest(
                "Complete the previous steps before submitting pricing.");
        if (draft.BrandId is null || draft.ModelId is null || draft.Year is null)
            return AppConc.Response<DraftStepResponse>.BadRequest(
                "Car details are incomplete. Resubmit the details step.");

        draft.SetPricing(command.Price, command.Description);

        var car = new Car(
            draft.BrandId.Value,
            draft.ModelId.Value,
            draft.Year.Value,
            draft.FuelType!.Value,
            draft.TransmissionType!.Value,
            draft.Mileage!.Value,
            command.Price,
            command.Description);

        await db.Cars.AddAsync(car, ct);
        draft.Complete();
        await db.SaveChangesAsync(ct);

        await publishEndpoint.Publish(
            new CarListingPublishedIntegrationEvent(car.Id, command.DraftId), ct);

        return AppConc.Response<DraftStepResponse>.Created(
            new DraftStepResponse(command.DraftId, 3, 3, null, true, car.Id));
    }
}
