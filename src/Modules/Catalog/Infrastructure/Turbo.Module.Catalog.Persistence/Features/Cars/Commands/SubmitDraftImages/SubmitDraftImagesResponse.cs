namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftImages;

/// <summary>Returned after images are uploaded; guides the client to the details step.</summary>
public sealed record SubmitDraftImagesResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    string NextStepKey
);