using MassTransit;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Media.Domain.Entity;
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

        var draftImages = await db.CarDraftImages
            .Where(x => x.DraftId == message.DraftId)
            .ToListAsync(ct);

        if (draftImages.Count == 0)
            return;

        var carImages = draftImages
            .Select(di => new CarImage(message.CarId, di.Url, di.ObjectKey, di.Order))
            .ToList();

        db.CarImages.AddRange(carImages);
        db.CarDraftImages.RemoveRange(draftImages);
        await db.SaveChangesAsync(ct);
    }
}
