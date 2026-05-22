namespace Turbo.Shared.Contracts.IntegrationEvents;

public sealed record CarListingPublishedIntegrationEvent(Guid CarId, Guid DraftId);
