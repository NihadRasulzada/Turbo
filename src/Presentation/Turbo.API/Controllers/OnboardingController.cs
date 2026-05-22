using Microsoft.AspNetCore.Mvc;
using Turbo.API.Controllers.Requests;
using Turbo.API.Extensions;
using Turbo.Module.Catalog.Persistence.Features.Onboarding;
using Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.CreateDraft;
using Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftDetails;
using Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftImages;
using Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftPricing;
using Turbo.Module.Catalog.Persistence.Features.Onboarding.Queries.GetDraft;
using Turbo.Module.Catalog.Persistence.Features.Onboarding.Queries.GetOnboardingConfig;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Web.Controllers;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

/// <summary>
/// Backend-driven multi-step car listing onboarding.
/// </summary>
[ApiController]
[Route("api/onboarding")]
[Produces("application/json")]
public sealed class OnboardingController(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher) : ControllerBase
{
    /// <summary>
    /// Returns the step definitions for the onboarding flow.
    /// </summary>
    /// <remarks>
    /// Clients use this endpoint to build the form UI dynamically.
    /// Includes available enum options for brand, model, fuel type, and transmission.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(SuccessResponse<GetOnboardingConfigResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfig(CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetOnboardingConfigRequest, AppConc.Response<GetOnboardingConfigResponse>>(
                new GetOnboardingConfigRequest(), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Creates a new draft and returns its ID.
    /// </summary>
    /// <remarks>Call this once at the beginning of the onboarding flow before submitting any step.</remarks>
    [HttpPost("drafts")]
    [ProducesResponseType(typeof(CreatedResponse<CreateDraftResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServerErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateDraft(CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<CreateDraftRequest, AppConc.Response<CreateDraftResponse>>(
                new CreateDraftRequest(), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Returns the current state of a draft (resume support).
    /// </summary>
    [HttpGet("drafts/{draftId:guid}")]
    [ProducesResponseType(typeof(SuccessResponse<GetDraftResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDraft([FromRoute] Guid draftId, CancellationToken ct)
    {
        var result = await queryDispatcher
            .DispatchAsync<GetDraftRequest, AppConc.Response<GetDraftResponse>>(
                new GetDraftRequest(draftId), ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Step 1 — upload car images.
    /// </summary>
    /// <remarks>
    /// Images are uploaded to object storage asynchronously after this call returns.
    /// Supported formats: JPEG, PNG, WebP, GIF.
    /// </remarks>
    [HttpPost("drafts/{draftId:guid}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SuccessResponse<DraftStepResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitImages(
        [FromRoute] Guid draftId,
        [FromForm] SubmitDraftImagesHttpRequest request,
        CancellationToken ct)
    {
        var command = new SubmitDraftImagesRequest(draftId, await request.Images.ToImageDataAsync(ct));
        var result = await commandDispatcher
            .DispatchAsync<SubmitDraftImagesRequest, AppConc.Response<DraftStepResponse>>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Step 2 — submit car details (brand, model, year, mileage, fuel type, transmission).
    /// </summary>
    [HttpPost("drafts/{draftId:guid}/details")]
    [ProducesResponseType(typeof(SuccessResponse<DraftStepResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitDetails(
        [FromRoute] Guid draftId,
        [FromBody] SubmitDraftDetailsHttpRequest request,
        CancellationToken ct)
    {
        var command = new SubmitDraftDetailsRequest(
            draftId, request.BrandId, request.ModelId, request.Year,
            request.FuelType, request.TransmissionType, request.Mileage);
        var result = await commandDispatcher
            .DispatchAsync<SubmitDraftDetailsRequest, AppConc.Response<DraftStepResponse>>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Step 3 — submit price and description, publishes the car listing.
    /// </summary>
    /// <remarks>
    /// This is the final step. On success, the draft is completed and a Car listing is created.
    /// The response includes the new <c>carId</c>.
    /// </remarks>
    [HttpPost("drafts/{draftId:guid}/pricing")]
    [ProducesResponseType(typeof(CreatedResponse<DraftStepResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitPricing(
        [FromRoute] Guid draftId,
        [FromBody] SubmitDraftPricingHttpRequest request,
        CancellationToken ct)
    {
        var command = new SubmitDraftPricingRequest(draftId, request.Price, request.Description);
        var result = await commandDispatcher
            .DispatchAsync<SubmitDraftPricingRequest, AppConc.Response<DraftStepResponse>>(command, ct);
        return this.HandleServiceResponse(result);
    }
}
