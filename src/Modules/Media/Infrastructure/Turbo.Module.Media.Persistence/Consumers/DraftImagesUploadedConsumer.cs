using MassTransit;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Domain.Entity;
using Turbo.Module.Media.Persistence.Contexts;
using Turbo.Shared.Contracts.IntegrationEvents;

namespace Turbo.Module.Media.Persistence.Consumers;

public sealed class DraftImagesUploadedConsumer(CommandDbContext db, IMinioService minioService)
    : IConsumer<DraftImagesUploadedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DraftImagesUploadedIntegrationEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        await minioService.EnsureBucketExistsAsync(ct);

        var draftImages = new List<CarDraftImage>(message.Images.Count);
        foreach (var image in message.Images)
        {
            var objectKey = $"drafts/{message.DraftId}/{Guid.NewGuid()}_{image.FileName}";
            using var stream = new MemoryStream(image.Data);
            var url = await minioService.UploadAsync(objectKey, stream, image.ContentType, ct);
            draftImages.Add(new CarDraftImage(message.DraftId, url, objectKey, image.Order));
        }

        db.CarDraftImages.AddRange(draftImages);
        await db.SaveChangesAsync(ct);
    }
}
