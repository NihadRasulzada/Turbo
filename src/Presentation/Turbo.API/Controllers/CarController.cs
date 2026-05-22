using Microsoft.AspNetCore.Mvc;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

[ApiController]
[Route("api/cars")]
public sealed class CarController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddCarRequest request, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync<AddCarRequest, Response<AddCarResponse>>(
            request,
            ct
        );

        return this.HandleServiceResponse(result);
    }
}
