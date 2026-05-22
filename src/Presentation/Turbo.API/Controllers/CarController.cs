using Microsoft.AspNetCore.Mvc;
using Turbo.API.Controllers.Requests;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

[ApiController]
[Route("api/cars")]
public sealed class CarController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Add([FromForm] AddCarHttpRequest request, CancellationToken ct)
    {
        var command = new AddCarRequest(
            request.Brand,
            request.Model,
            request.Year,
            request.FuelType,
            request.TransmissionType,
            request.Mileage,
            request.Price,
            request.Description,
            await request.Images.ToImageDataAsync(ct)
        );

        var result = await dispatcher.DispatchAsync<AddCarRequest, AppConc.Response<AddCarResponse>>(
            command,
            ct
        );

        return this.HandleServiceResponse(result);
    }
}
