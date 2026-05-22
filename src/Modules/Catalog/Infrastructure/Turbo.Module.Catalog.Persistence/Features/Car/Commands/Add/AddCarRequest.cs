using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;

public sealed record AddCarRequest(
    Brand Brand,
    Model Model,
    short Year,
    FuelType FuelType,
    TransmissionType TransmissionType,
    int Mileage
) : ICommand<Response<AddCarResponse>>;