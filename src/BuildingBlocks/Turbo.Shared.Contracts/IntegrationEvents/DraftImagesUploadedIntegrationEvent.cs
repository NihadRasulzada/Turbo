using Turbo.Shared.Contracts.Dtos;

namespace Turbo.Shared.Contracts.IntegrationEvents;

public sealed record DraftImagesUploadedIntegrationEvent(
    Guid DraftId,
    IReadOnlyList<ImageData> Images
);