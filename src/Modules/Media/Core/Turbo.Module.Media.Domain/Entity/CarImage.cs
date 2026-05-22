using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Media.Domain.Entity;

public class CarImage : BaseEntity
{
    public Guid CarId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool IsResized { get; private set; }

    public CarImage(Guid carId, string url, string objectKey, int order)
        : base(Guid.NewGuid())
    {
        CarId = carId;
        Url = url;
        ObjectKey = objectKey;
        Order = order;
        IsResized = false;
    }

    public void SetResized() => IsResized = true;

    protected CarImage()
        : base(Guid.NewGuid()) { } // EF Core
}
