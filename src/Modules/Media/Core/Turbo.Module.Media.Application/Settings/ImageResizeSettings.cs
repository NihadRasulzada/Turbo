namespace Turbo.Module.Media.Application.Settings;

public sealed class ImageResizeSettings
{
    public int MaxWidth { get; set; } = 1920;
    public int MaxHeight { get; set; } = 1080;
    public int BatchSize { get; set; } = 10;
    public int PollingIntervalSeconds { get; set; } = 30;
}
