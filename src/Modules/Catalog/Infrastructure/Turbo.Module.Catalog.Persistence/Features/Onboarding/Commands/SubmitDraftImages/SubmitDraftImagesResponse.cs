namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftImages;

/// <summary>Returned after images are uploaded; guides the client to the details step.</summary>
public sealed record SubmitDraftImagesResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    string NextStepKey
);
