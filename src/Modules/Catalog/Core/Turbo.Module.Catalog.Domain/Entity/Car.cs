using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Domain.Exceptions;
using Turbo.Module.Catalog.Domain.Extensions;
using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Catalog.Domain.Entity;

public class Car : BaseEntity
{
    private Brand _brand;
    private Model _model;
    private short _year;
    private FuelType _fuelType;
    private TransmissionType _transmissionType;
    private int _mileage;
    private int _price;
    private string _description = string.Empty;

    public Brand Brand
    {
        get => _brand;
        private set => _brand = value;
    }

    public Model Model
    {
        get => _model;
        private set
        {
            if (!value.BelongsTo(_brand))
                throw new DomainException($"{value} does not belong to {_brand}.");
            _model = value;
        }
    }

    public short Year
    {
        get => _year;
        private set
        {
            if (value < 1886 || value > (short)DateTime.UtcNow.Year)
                throw new DomainException($"Year must be between 1886 and {DateTime.UtcNow.Year}.");
            _year = value;
        }
    }

    public FuelType FuelType
    {
        get => _fuelType;
        private set
        {
            if (!System.Enum.IsDefined(value))
                throw new DomainException($"Invalid fuel type: {value}.");
            _fuelType = value;
        }
    }

    public TransmissionType TransmissionType
    {
        get => _transmissionType;
        private set
        {
            if (!System.Enum.IsDefined(value))
                throw new DomainException($"Invalid transmission type: {value}.");
            _transmissionType = value;
        }
    }

    public int Mileage
    {
        get => _mileage;
        private set
        {
            if (value < 0)
                throw new DomainException("Mileage cannot be negative.");
            _mileage = value;
        }
    }

    public int Price
    {
        get => _price;
        private set
        {
            if (value <= 0)
                throw new DomainException("Price must be greater than zero.");
            _price = value;
        }
    }

    public string Description
    {
        get => _description;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Description cannot be empty.");
            if (value.Length > 2000)
                throw new DomainException("Description must not exceed 2000 characters.");
            _description = value;
        }
    }

    public Car(
        Brand brand,
        Model model,
        short year,
        FuelType fuelType,
        TransmissionType transmissionType,
        int mileage,
        int price,
        string description
    )
        : base(Guid.NewGuid())
    {
        Brand = brand;
        Model = model;
        Year = year;
        FuelType = fuelType;
        TransmissionType = transmissionType;
        Mileage = mileage;
        Price = price;
        Description = description;
    }

    protected Car()
        : base(Guid.NewGuid()) { } // EF Core
}
