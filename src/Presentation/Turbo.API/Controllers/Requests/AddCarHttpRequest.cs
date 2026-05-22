using Microsoft.AspNetCore.Http;
using Turbo.Module.Catalog.Domain.Enum;

namespace Turbo.API.Controllers.Requests;

/// <summary>Multipart form body for creating a new car listing.</summary>
public sealed class AddCarHttpRequest
{
    /// <summary>Manufacturer brand (e.g. Toyota, BMW).</summary>
    /// <example>1</example>
    public Brand Brand { get; set; }

    /// <summary>Specific model belonging to the selected brand.</summary>
    /// <example>2</example>
    public Model Model { get; set; }

    /// <summary>Production year (1886 – current year).</summary>
    /// <example>2022</example>
    public short Year { get; set; }

    /// <summary>Fuel type (Petrol, Diesel, Electric, Hybrid, LPG).</summary>
    /// <example>0</example>
    public FuelType FuelType { get; set; }

    /// <summary>Transmission type (Manual, Automatic, SemiAutomatic).</summary>
    /// <example>1</example>
    public TransmissionType TransmissionType { get; set; }

    /// <summary>Total kilometres driven. Must be zero or greater.</summary>
    /// <example>45000</example>
    public int Mileage { get; set; }

    /// <summary>Asking price in the local currency. Must be greater than zero.</summary>
    /// <example>25000</example>
    public int Price { get; set; }

    /// <summary>Free-text description of the car (max 2000 characters).</summary>
    /// <example>Well-maintained, single owner, full service history.</example>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// One or more car images (JPEG, PNG, WebP or GIF).
    /// Images are stored in MinIO and automatically resized to 1920×1080 in the background.
    /// </summary>
    public IFormFileCollection? Images { get; set; }
}
