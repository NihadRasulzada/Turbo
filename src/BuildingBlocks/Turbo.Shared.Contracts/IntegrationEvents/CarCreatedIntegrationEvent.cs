using Turbo.Shared.Contracts.Dtos;

namespace Turbo.Shared.Contracts.IntegrationEvents;

public sealed record CarCreatedIntegrationEvent(Guid CarId, IReadOnlyList<ImageData> Images);
