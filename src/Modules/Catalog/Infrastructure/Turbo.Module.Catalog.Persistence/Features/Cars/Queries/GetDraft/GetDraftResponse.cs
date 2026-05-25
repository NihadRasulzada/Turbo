using Turbo.Module.Catalog.Domain.Enum;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;

public sealed record GetDraftResponse(
    Guid DraftId,
    CarDraftStatus Status,
    int CurrentStep,
    int TotalSteps
);