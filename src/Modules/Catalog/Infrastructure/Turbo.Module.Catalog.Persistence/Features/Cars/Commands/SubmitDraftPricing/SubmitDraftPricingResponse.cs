namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;

/// <summary>Returned when the draft is published; contains the new car's ID.</summary>
public sealed record SubmitDraftPricingResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    Guid CarId
);
