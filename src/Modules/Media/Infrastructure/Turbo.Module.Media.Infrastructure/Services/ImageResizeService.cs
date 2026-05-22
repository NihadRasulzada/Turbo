using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Turbo.Module.Media.Application.Interfaces;

namespace Turbo.Module.Media.Infrastructure.Services;

public sealed class ImageResizeService : IImageResizeService
{
    public async Task<(byte[] Data, string ContentType)> ResizeAsync(
        byte[] imageData,
        string contentType,
        int maxWidth,
        int maxHeight,
        CancellationToken ct = default
    )
    {
        using var inputStream = new MemoryStream(imageData);
        using var image = await Image.LoadAsync(inputStream, ct);

        if (image.Width <= maxWidth && image.Height <= maxHeight)
            return (imageData, contentType);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(maxWidth, maxHeight),
            Mode = ResizeMode.Max
        }));

        var encoder = contentType switch
        {
            "image/png" => (SixLabors.ImageSharp.Formats.IImageEncoder)new PngEncoder(),
            "image/gif" => new GifEncoder(),
            "image/webp" => new WebpEncoder(),
            _ => new JpegEncoder()
        };

        using var output = new MemoryStream();
        await image.SaveAsync(output, encoder, ct);
        return (output.ToArray(), contentType);
    }
}
