using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Media.Domain.Entity;

public class CarImage : BaseEntity
{
    public Guid CarId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CarImage(Guid carId, string url, int order)
        : base(Guid.NewGuid())
    {
        CarId = carId;
        Url = url;
        Order = order;
        CreatedAt = DateTime.UtcNow;
    }

    protected CarImage()
        : base(Guid.NewGuid()) { } // EF Core
}
