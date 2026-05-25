using MassTransit;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Domain.Enums;
using Turbo.Module.Media.Persistence.Contexts;
using Turbo.Shared.Contracts.IntegrationEvents;
using MediaEntity = Turbo.Module.Media.Domain.Entity.Media;

namespace Turbo.Module.Media.Persistence.Consumers;

public sealed class DraftImagesUploadedConsumer(IMediaWriteDbContext db, IMinioService minioService)
    : IConsumer<DraftImagesUploadedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DraftImagesUploadedIntegrationEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        await minioService.EnsureBucketExistsAsync(ct);

        var mediaItems = new List<MediaEntity>(message.Images.Count);
        foreach (var image in message.Images)
        {
            var objectKey = $"drafts/{message.DraftId}/{Guid.NewGuid()}_{image.FileName}";
            using var stream = new MemoryStream(image.Data);
            var url = await minioService.UploadAsync(objectKey, stream, image.ContentType, ct);
            mediaItems.Add(new MediaEntity(message.DraftId, MediaOwnerType.CarDraft, url, objectKey, image.Order));
        }

        db.AddRange(mediaItems);
        await db.SaveChangesAsync(ct);
    }
}
