using Microsoft.AspNetCore.Http;
using Turbo.Shared.Contracts.Dtos;

namespace Turbo.API.Extensions;

public static class FormFileExtensions
{
    public static async Task<IReadOnlyList<ImageData>> ToImageDataAsync(
        this IFormFileCollection? files,
        CancellationToken ct = default
    )
    {
        if (files is null || files.Count == 0)
            return [];

        var result = new List<ImageData>(files.Count);
        int order = 0;
        foreach (var file in files)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            result.Add(new ImageData(file.FileName, file.ContentType, ms.ToArray(), order++));
        }
        return result;
    }
}
