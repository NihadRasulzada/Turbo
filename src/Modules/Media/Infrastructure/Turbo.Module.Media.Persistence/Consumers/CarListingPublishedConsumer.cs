using MassTransit;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Media.Domain.Enums;
using Turbo.Module.Media.Persistence.Contexts;
using Turbo.Shared.Contracts.IntegrationEvents;
using MediaEntity = Turbo.Module.Media.Domain.Entity.Media;

namespace Turbo.Module.Media.Persistence.Consumers;

public sealed class CarListingPublishedConsumer(
    IMediaWriteDbContext writeDb,
    IMediaReadDbContext readDb)
    : IConsumer<CarListingPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CarListingPublishedIntegrationEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        // Load untracked from read DB; attach to write DB so mutations are persisted.
        var draftMediaItems = await readDb.Medias
            .AsNoTracking()
            .Where(m => m.OwnerId == message.DraftId && m.OwnerType == MediaOwnerType.CarDraft)
            .ToListAsync(ct);

        if (draftMediaItems.Count == 0)
            return;

        writeDb.AttachRange(draftMediaItems);

        foreach (var media in draftMediaItems)
            media.TransferOwnership(message.CarId, MediaOwnerType.Car);

        await writeDb.SaveChangesAsync(ct);
    }
}