using Microsoft.AspNetCore.Mvc;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;
using Turbo.Module.Catalog.Persistence.Features.Model.Commands.DeleteModel;
using Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;
using Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetAllModels;
using Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Web.Controllers;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

/// <summary>
/// CRUD operations for car models.
/// </summary>
[ApiController]
[Route("api/models")]
[Produces("application/json")]
public sealed class ModelController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher) : ControllerBase
{
    /// <summary>Returns all models, optionally filtered by brand.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<IReadOnlyList<GetAllModelsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? brandId, CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetAllModelsRequest, AppConc.Response<IReadOnlyList<GetAllModelsResponse>>>(
                new GetAllModelsRequest(brandId), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Returns a single model by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<GetModelByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetModelByIdRequest, AppConc.Response<GetModelByIdResponse>>(
                new GetModelByIdRequest(id), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Creates a new model under a brand.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse<CreateModelResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateModelRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<CreateModelRequest, AppConc.Response<CreateModelResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Updates an existing model.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<UpdateModelResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateModelHttpRequest request,
        CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<UpdateModelRequest, AppConc.Response<UpdateModelResponse>>(
                new UpdateModelRequest(id, request.Name, request.BrandId), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Deletes a model. Fails if any cars reference this model.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<DeleteModelRequest, AppConc.Response>(
                new DeleteModelRequest(id), ct);
        return this.HandleServiceResponse(result);
    }
}

/// <summary>Model update payload.</summary>
public sealed class UpdateModelHttpRequest
{
    /// <example>Camry</example>
    public string Name { get; set; } = string.Empty;
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid BrandId { get; set; }
}