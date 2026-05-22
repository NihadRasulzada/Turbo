using Microsoft.AspNetCore.Mvc;
using Turbo.API.Controllers.Requests;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;
using Turbo.Shared.Contracts.Dtos;

namespace Turbo.API.Controllers;

[ApiController]
[Route("api/cars")]
public sealed class CarController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Add([FromForm] AddCarHttpRequest request, CancellationToken ct)
    {
        var images = new List<ImageData>();
        if (request.Images is not null)
        {
            int order = 0;
            foreach (var file in request.Images)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms, ct);
                images.Add(new ImageData(file.FileName, file.ContentType, ms.ToArray(), order++));
            }
        }

        var command = new AddCarRequest(
            request.Brand,
            request.Model,
            request.Year,
            request.FuelType,
            request.TransmissionType,
            request.Mileage,
            request.Price,
            request.Description,
            images
        );

        var result = await dispatcher.DispatchAsync<AddCarRequest, Response<AddCarResponse>>(
            command,
            ct
        );

        return this.HandleServiceResponse(result);
    }
}
