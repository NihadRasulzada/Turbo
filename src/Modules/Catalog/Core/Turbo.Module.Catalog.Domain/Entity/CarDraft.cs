using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Catalog.Domain.Entity;

public class CarDraft : BaseEntity
{
    public Guid? SellerId { get; private set; }
    public CarDraftStatus Status { get; private set; }
    public int CurrentStep { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Step 2 — car details
    public Guid? BrandId { get; private set; }
    public Guid? ModelId { get; private set; }
    public short? Year { get; private set; }
    public FuelType? FuelType { get; private set; }
    public TransmissionType? TransmissionType { get; private set; }
    public int? Mileage { get; private set; }

    // Step 3 — pricing
    public int? Price { get; private set; }
    public string? Description { get; private set; }

    // ── Factory ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Yeni draft yaradır. SellerId-nin token-dən gəlməsi təmin edilir.
    /// </summary>
    public static CarDraft Create(Guid sellerId) =>
        new(Guid.NewGuid(), (Guid?)sellerId);

    // Two-arg constructor avoids signature clash with EF Core's single-Guid ctor.
    private CarDraft(Guid id, Guid? sellerId) : base(id)
    {
        SellerId = sellerId;
        Status = CarDraftStatus.InProgress;
        CurrentStep = 1;
        CreatedAt = DateTime.UtcNow;
    }

    protected CarDraft(Guid id) : base(id) { } // EF Core materialisation

    public void AdvanceStep() => CurrentStep++;

    public void SetDetails(
        Guid brandId,
        Guid modelId,
        short year,
        FuelType fuelType,
        TransmissionType transmissionType,
        int mileage)
    {
        BrandId = brandId;
        ModelId = modelId;
        Year = year;
        FuelType = fuelType;
        TransmissionType = transmissionType;
        Mileage = mileage;
    }

    public void SetPricing(int price, string description)
    {
        Price = price;
        Description = description;
    }

    public void Complete() => Status = CarDraftStatus.Completed;
}
