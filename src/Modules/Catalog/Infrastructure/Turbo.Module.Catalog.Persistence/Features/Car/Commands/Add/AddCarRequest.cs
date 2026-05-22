using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;
using Turbo.Shared.Contracts.Dtos;

namespace Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;

public sealed record AddCarRequest(
    Brand Brand,
    Model Model,
    short Year,
    FuelType FuelType,
    TransmissionType TransmissionType,
    int Mileage,
    int Price,
    string Description,
    IReadOnlyList<ImageData> Images
) : ICommand<Response<AddCarResponse>>;
