using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Turbo.Module.Media.Application.Interfaces;
using Turbo.Module.Media.Infrastructure.Settings;

namespace Turbo.Module.Media.Infrastructure.Services;

public sealed class MinioService(IMinioClient minioClient, IOptions<MinioSettings> settings)
    : IMinioService
{
    private readonly string _bucket = settings.Value.BucketName;
    private readonly MinioSettings _settings = settings.Value;

    public async Task EnsureBucketExistsAsync(CancellationToken ct = default)
    {
        var exists = await minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucket),
            ct
        );

        if (!exists)
            await minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucket),
                ct
            );
    }

    public async Task<string> UploadAsync(
        string objectKey,
        Stream data,
        string contentType,
        CancellationToken ct = default
    )
    {
        var args = new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey)
            .WithStreamData(data)
            .WithObjectSize(data.Length)
            .WithContentType(contentType);

        await minioClient.PutObjectAsync(args, ct);

        var scheme = _settings.UseSSL ? "https" : "http";
        return $"{scheme}://{_settings.Endpoint}/{_bucket}/{objectKey}";
    }

    public async Task<byte[]> DownloadAsync(string objectKey, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectKey)
            .WithCallbackStream(async (stream, cancellationToken) =>
                await stream.CopyToAsync(ms, cancellationToken)
            );

        await minioClient.GetObjectAsync(args, ct);
        return ms.ToArray();
    }
}
