using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Application.Settings;
using Turbo.Module.Media.Persistence.Contexts;
using MediaEntity = Turbo.Module.Media.Domain.Entity.Media;

namespace Turbo.Module.Media.Persistence.BackgroundServices;

public sealed class ImageResizeBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<ImageResizeSettings> settings,
    ILogger<ImageResizeBackgroundService> logger
) : BackgroundService
{
    private readonly ImageResizeSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in image resize background service");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(_settings.PollingIntervalSeconds),
                stoppingToken
            );
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IMediaWriteDbContext>();
        var minioService = scope.ServiceProvider.GetRequiredService<IMinioService>();
        var resizeService = scope.ServiceProvider.GetRequiredService<IImageResizeService>();

        // Load via write context so mutations are tracked before SaveChangesAsync.
        var pending = await db.Set<MediaEntity>()
            .Where(m => !m.IsResized)
            .Take(_settings.BatchSize)
            .ToListAsync(ct);

        if (pending.Count == 0)
            return;

        logger.LogInformation("Resizing {Count} pending media item(s)", pending.Count);

        foreach (var media in pending)
        {
            try
            {
                var original = await minioService.DownloadAsync(media.ObjectKey, ct);

                var ext = Path.GetExtension(media.ObjectKey).TrimStart('.').ToLowerInvariant();
                var originalContentType = ext switch
                {
                    "png"  => "image/png",
                    "gif"  => "image/gif",
                    "webp" => "image/webp",
                    _      => "image/jpeg"
                };

                var (resized, contentType) = await resizeService.ResizeAsync(
                    original,
                    originalContentType,
                    _settings.MaxWidth,
                    _settings.MaxHeight,
                    ct
                );

                using var stream = new MemoryStream(resized);
                await minioService.UploadAsync(media.ObjectKey, stream, contentType, ct);

                media.SetResized();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to resize media {MediaId} (ObjectKey: {ObjectKey})",
                    media.Id, media.ObjectKey);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
