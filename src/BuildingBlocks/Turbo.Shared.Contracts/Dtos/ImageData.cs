namespace Turbo.Shared.Contracts.Dtos;

public sealed record ImageData(string FileName, string ContentType, byte[] Data, int Order);