using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;

public sealed class AddCarHandler(CommandDbContext db)
    : ICommandHandler<AddCarRequest, Response<AddCarResponse>>
{
    public async Task<Response<AddCarResponse>> HandleAsync(
        AddCarRequest command,
        CancellationToken ct = default
    )
    {
        var car = new Domain.Entity.Car(
            command.Brand,
            command.Model,
            command.Year,
            command.FuelType,
            command.TransmissionType,
            command.Mileage
        );

        await db.Cars.AddAsync(car, ct);
        await db.SaveChangesAsync(ct);

        return Response<AddCarResponse>.Created(new AddCarResponse(car.Id));
    }
}
