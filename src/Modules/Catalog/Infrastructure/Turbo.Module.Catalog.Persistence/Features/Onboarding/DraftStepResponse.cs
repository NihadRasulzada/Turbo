namespace Turbo.Module.Catalog.Persistence.Features.Onboarding;

/// <summary>Returned after each step submission to guide the client to the next step.</summary>
public sealed record DraftStepResponse(
    Guid DraftId,
    int CompletedStep,
    int TotalSteps,
    string? NextStepKey,
    bool IsComplete,
    Guid? CarId = null
);