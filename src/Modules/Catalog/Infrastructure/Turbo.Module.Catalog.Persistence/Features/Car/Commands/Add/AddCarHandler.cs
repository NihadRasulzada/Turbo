using MassTransit;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Contracts.IntegrationEvents;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;

public sealed class AddCarHandler(CommandDbContext db, IPublishEndpoint publishEndpoint)
    : ICommandHandler<AddCarRequest, AppConc.Response<AddCarResponse>>
{
    public async Task<AppConc.Response<AddCarResponse>> HandleAsync(
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
            command.Mileage,
            command.Price,
            command.Description
        );

        await db.Cars.AddAsync(car, ct);
        await db.SaveChangesAsync(ct);

        if (command.Images.Count > 0)
        {
            await publishEndpoint.Publish(
                new CarCreatedIntegrationEvent(car.Id, command.Images),
                ct
            );
        }

        return AppConc.Response<AddCarResponse>.Created(new AddCarResponse(car.Id));
    }
}
