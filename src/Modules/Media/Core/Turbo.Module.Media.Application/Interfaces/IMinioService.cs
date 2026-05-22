namespace Turbo.Module.Media.Application.Interfaces;

public interface IMinioService
{
    Task<string> UploadAsync(string objectKey, Stream data, string contentType, CancellationToken ct = default);
    Task<byte[]> DownloadAsync(string objectKey, CancellationToken ct = default);
    Task EnsureBucketExistsAsync(CancellationToken ct = default);
}
