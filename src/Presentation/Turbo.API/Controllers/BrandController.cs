using Microsoft.AspNetCore.Mvc;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Brand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;
using Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;
using Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetBrandById;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Web.Controllers;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

/// <summary>
/// CRUD operations for car brands.
/// </summary>
[ApiController]
[Route("api/brands")]
[Produces("application/json")]
public sealed class BrandController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher) : ControllerBase
{
    /// <summary>Returns all brands ordered by name.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<IReadOnlyList<BrandResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetAllBrandsRequest, AppConc.Response<IReadOnlyList<BrandResponse>>>(
                new GetAllBrandsRequest(), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Returns a single brand by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<BrandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetBrandByIdRequest, AppConc.Response<BrandResponse>>(
                new GetBrandByIdRequest(id), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Creates a new brand.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse<BrandResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<CreateBrandRequest, AppConc.Response<BrandResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Updates an existing brand.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<BrandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandHttpRequest request,
        CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<UpdateBrandRequest, AppConc.Response<BrandResponse>>(
                new UpdateBrandRequest(id, request.Name), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>Deletes a brand. Fails if any models reference this brand.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ConflictResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<DeleteBrandRequest, AppConc.Response>(
                new DeleteBrandRequest(id), ct);
        return this.HandleServiceResponse(result);
    }
}

/// <summary>Brand name update payload.</summary>
public sealed class UpdateBrandHttpRequest
{
    /// <example>Toyota</example>
    public string Name { get; set; } = string.Empty;
}