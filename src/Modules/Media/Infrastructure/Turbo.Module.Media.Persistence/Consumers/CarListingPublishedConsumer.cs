using MassTransit;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Media.Domain.Enums;
using Turbo.Module.Media.Persistence.Contexts;
using Turbo.Shared.Contracts.IntegrationEvents;

namespace Turbo.Module.Media.Persistence.Consumers;

public sealed class CarListingPublishedConsumer(CommandDbContext db)
    : IConsumer<CarListingPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CarListingPublishedIntegrationEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        var draftMediaItems = await db.Medias
            .Where(m => m.OwnerId == message.DraftId && m.OwnerType == MediaOwnerType.CarDraft)
            .ToListAsync(ct);

        if (draftMediaItems.Count == 0)
            return;

        foreach (var media in draftMediaItems)
            media.TransferOwnership(message.CarId, MediaOwnerType.Car);

        await db.SaveChangesAsync(ct);
    }
}
