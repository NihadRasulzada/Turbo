using Microsoft.AspNetCore.Mvc;
using Turbo.API.Controllers.Requests;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Web.Controllers;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

/// <summary>
/// Manages car listings in the marketplace.
/// </summary>
[ApiController]
[Route("api/cars")]
[Produces("application/json")]
public sealed class CarController(ICommandDispatcher dispatcher) : ControllerBase
{
    /// <summary>
    /// Create a new car listing.
    /// </summary>
    /// <remarks>
    /// Submits a multipart/form-data request containing car details and one or more images.
    /// Images are uploaded to object storage asynchronously after the car record is saved,
    /// and are resized in the background to a maximum of 1920×1080.
    /// </remarks>
    /// <param name="request">Car details and image files.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The ID of the newly created car listing.</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(CreatedResponse<AddCarResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ServerErrorResponse), StatusCodes.Status500InternalServerError)]
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
