using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Domain.Exceptions;
using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Catalog.Domain.Entity;

public class Car : BaseEntity
{
    public Guid BrandId { get; private set; }
    public Guid ModelId { get; private set; }
    public short Year { get; private set; }
    public FuelType FuelType { get; private set; }
    public TransmissionType TransmissionType { get; private set; }
    public int Mileage { get; private set; }
    public int Price { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public Car(
        Guid brandId,
        Guid modelId,
        short year,
        FuelType fuelType,
        TransmissionType transmissionType,
        int mileage,
        int price,
        string description)
        : base(Guid.NewGuid())
    {
        if (brandId == Guid.Empty) throw new DomainException("BrandId cannot be empty.");
        if (modelId == Guid.Empty) throw new DomainException("ModelId cannot be empty.");
        if (year < 1886 || year > (short)DateTime.UtcNow.Year)
            throw new DomainException($"Year must be between 1886 and {DateTime.UtcNow.Year}.");
        if (!System.Enum.IsDefined(fuelType))
            throw new DomainException($"Invalid fuel type: {fuelType}.");
        if (!System.Enum.IsDefined(transmissionType))
            throw new DomainException($"Invalid transmission type: {transmissionType}.");
        if (mileage < 0) throw new DomainException("Mileage cannot be negative.");
        if (price <= 0) throw new DomainException("Price must be greater than zero.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description cannot be empty.");
        if (description.Length > 2000) throw new DomainException("Description must not exceed 2000 characters.");

        BrandId = brandId;
        ModelId = modelId;
        Year = year;
        FuelType = fuelType;
        TransmissionType = transmissionType;
        Mileage = mileage;
        Price = price;
        Description = description;
    }

    protected Car() : base(Guid.NewGuid()) { } // EF Core
}