using MassTransit;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Domain.Entity;
using Turbo.Module.Media.Persistence.Contexts;
using Turbo.Shared.Contracts.IntegrationEvents;

namespace Turbo.Module.Media.Persistence.Consumers;

public sealed class CarCreatedEventConsumer(CommandDbContext db, IMinioService minioService)
    : IConsumer<CarCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CarCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        var ct = context.CancellationToken;

        await minioService.EnsureBucketExistsAsync(ct);

        var carImages = new List<CarImage>(message.Images.Count);
        foreach (var image in message.Images)
        {
            var objectKey = $"cars/{message.CarId}/{Guid.NewGuid()}_{image.FileName}";
            using var stream = new MemoryStream(image.Data);
            var url = await minioService.UploadAsync(objectKey, stream, image.ContentType, ct);
            carImages.Add(new CarImage(message.CarId, url, objectKey, image.Order));
        }

        db.CarImages.AddRange(carImages);
        await db.SaveChangesAsync(ct);
    }
}
