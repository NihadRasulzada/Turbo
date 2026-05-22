using Microsoft.AspNetCore.Http;
using Turbo.Module.Catalog.Domain.Enum;

namespace Turbo.API.Controllers.Requests;

public sealed class AddCarHttpRequest
{
    public Brand Brand { get; set; }
    public Model Model { get; set; }
    public short Year { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType TransmissionType { get; set; }
    public int Mileage { get; set; }
    public int Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public IFormFileCollection? Images { get; set; }
}
