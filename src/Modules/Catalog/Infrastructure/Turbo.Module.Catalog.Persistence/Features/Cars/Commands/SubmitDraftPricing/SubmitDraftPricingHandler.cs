using MassTransit;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Contracts.IntegrationEvents;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;

public sealed class SubmitDraftPricingHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<SubmitDraftPricingRequest, AppConc.Response<SubmitDraftPricingResponse>>
{
    public async Task<AppConc.Response<SubmitDraftPricingResponse>> HandleAsync(
        SubmitDraftPricingRequest command,
        CancellationToken ct = default)
    {
        var draft = await readDb.CarDrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == command.DraftId, ct);
        if (draft is null)
            return AppConc.Response<SubmitDraftPricingResponse>.NotFound("Draft not found.");
        if (draft.Status == CarDraftStatus.Completed)
            return AppConc.Response<SubmitDraftPricingResponse>.BadRequest("Draft is already completed.");
        if (draft.CurrentStep != 3)
            return AppConc.Response<SubmitDraftPricingResponse>.BadRequest(
                "Complete the previous steps before submitting pricing.");
        if (draft.BrandId is null || draft.ModelId is null || draft.Year is null)
            return AppConc.Response<SubmitDraftPricingResponse>.BadRequest(
                "Car details are incomplete. Resubmit the details step.");

        var car = new Car(
            draft.BrandId.Value,
            draft.ModelId.Value,
            draft.Year.Value,
            draft.FuelType!.Value,
            draft.TransmissionType!.Value,
            draft.Mileage!.Value,
            command.Price,
            command.Description);

        writeDb.Attach(draft);
        draft.SetPricing(command.Price, command.Description);
        draft.Complete();
        writeDb.Add(car);
        await writeDb.SaveChangesAsync(ct);

        await publishEndpoint.Publish(
            new CarListingPublishedIntegrationEvent(car.Id, command.DraftId), ct);

        return AppConc.Response<SubmitDraftPricingResponse>.Created(
            new SubmitDraftPricingResponse(command.DraftId, 3, 3, car.Id));
    }
}
