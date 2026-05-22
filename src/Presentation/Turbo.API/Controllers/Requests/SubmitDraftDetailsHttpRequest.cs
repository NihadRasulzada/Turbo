using Turbo.Module.Catalog.Domain.Enum;

namespace Turbo.API.Controllers.Requests;

/// <summary>Car detail fields for step 2.</summary>
public sealed class SubmitDraftDetailsHttpRequest
{
    /// <example>1</example>
    public Brand Brand { get; set; }
    /// <example>2</example>
    public Model Model { get; set; }
    /// <example>2022</example>
    public short Year { get; set; }
    /// <example>0</example>
    public FuelType FuelType { get; set; }
    /// <example>1</example>
    public TransmissionType TransmissionType { get; set; }
    /// <example>45000</example>
    public int Mileage { get; set; }
}
