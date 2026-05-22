namespace Turbo.Module.Media.Application.Interfaces;

public interface IImageResizeService
{
    Task<(byte[] Data, string ContentType)> ResizeAsync(
        byte[] imageData,
        string contentType,
        int maxWidth,
        int maxHeight,
        CancellationToken ct = default
    );
}
