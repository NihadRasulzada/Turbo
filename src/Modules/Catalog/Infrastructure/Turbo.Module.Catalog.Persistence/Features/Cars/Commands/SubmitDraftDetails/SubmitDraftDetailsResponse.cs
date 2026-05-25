namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

/// <summary>Returned after details are saved; guides the client to the pricing step.</summary>
public sealed record SubmitDraftDetailsResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    string NextStepKey
);
