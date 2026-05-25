using Microsoft.AspNetCore.Mvc;
using Turbo.Shared.Application.ResponseObject.Concreate;
using Turbo.Shared.Application.ResponseObject.Enums;
using Turbo.Shared.Web.Controllers;

namespace Turbo.API.Extensions;

public static class ControllerExtensions
{
    /// <summary>
    /// Handles service response with data
    /// </summary>
    public static IActionResult HandleServiceResponse<T>(
        this ControllerBase controller,
        Response<T> response
    )
    {
        return response.ResponseStatusCode switch
        {
            ResponseStatusCode.Success => controller.Ok(
                new SuccessResponse<T>(
                    data: response.Data,
                    message: response.Message ?? "Operation completed successfully"
                )
            ),
            ResponseStatusCode.Created => controller.Created(
                string.Empty,
                new CreatedResponse<T>(
                    data: response.Data,
                    message: response.Message ?? "Resource created successfully"
                )
            ),
            ResponseStatusCode.NoContent => controller.NoContent(),
            ResponseStatusCode.BadRequest => controller.BadRequest(
                new BadRequestResponse(response.Message ?? "Bad request")
            ),
            ResponseStatusCode.NotFound => controller.NotFound(
                new NotFoundResponse(response.Message ?? "Resource not found")
            ),
            ResponseStatusCode.Unauthorized => controller.Unauthorized(
                new ErrorResponse(response.Message ?? "Unauthorized access")
            ),
            ResponseStatusCode.Forbidden => controller.StatusCode(
                403,
                new ErrorResponse(response.Message ?? "Forbidden")
            ),
            ResponseStatusCode.Conflict => controller.Conflict(
                new ConflictResponse(response.Message ?? "Conflict occurred")
            ),
            ResponseStatusCode.ValidationError => controller.UnprocessableEntity(
                new ValidationErrorResponse(
                    response.Message ?? "Validation failed",
                    (response.ValidationErrors ?? [])
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                )
            ),
            ResponseStatusCode.Error or ResponseStatusCode.InternalServerError =>
                controller.StatusCode(
                    500,
                    new ServerErrorResponse(
                        response.Message ?? "An internal server error occurred",
                        response.Errors
                    )
                ),
            _ => controller.StatusCode(
                500,
                new ServerErrorResponse("An unexpected error occurred", null)
            ),
        };
    }

    /// <summary>
    /// Handles service response without data
    /// </summary>
    public static IActionResult HandleServiceResponse(
        this ControllerBase controller,
        Response response
    )
    {
        return response.ResponseStatusCode switch
        {
            ResponseStatusCode.Success => controller.Ok(
                new SuccessResponse(message: response.Message ?? "Operation completed successfully")
            ),
            ResponseStatusCode.Created => controller.Created(
                string.Empty,
                new SuccessResponse(message: response.Message ?? "Resource created successfully")
            ),
            ResponseStatusCode.NoContent => controller.NoContent(),
            ResponseStatusCode.BadRequest => controller.BadRequest(
                new BadRequestResponse(response.Message ?? "Bad request")
            ),
            ResponseStatusCode.NotFound => controller.NotFound(
                new NotFoundResponse(response.Message ?? "Resource not found")
            ),
            ResponseStatusCode.Unauthorized => controller.Unauthorized(
                new ErrorResponse(response.Message ?? "Unauthorized access")
            ),
            ResponseStatusCode.Forbidden => controller.StatusCode(
                403,
                new ErrorResponse(response.Message ?? "Forbidden")
            ),
            ResponseStatusCode.Conflict => controller.Conflict(
                new ConflictResponse(response.Message ?? "Conflict occurred")
            ),
            ResponseStatusCode.ValidationError => controller.UnprocessableEntity(
                new ValidationErrorResponse(
                    response.Message ?? "Validation failed",
                    (response.ValidationErrors ?? [])
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                )
            ),
            ResponseStatusCode.Error or ResponseStatusCode.InternalServerError =>
                controller.StatusCode(
                    500,
                    new ServerErrorResponse(
                        response.Message ?? "An internal server error occurred",
                        response.Errors
                    )
                ),
            _ => controller.StatusCode(
                500,
                new ServerErrorResponse("An unexpected error occurred", null)
            ),
        };
    }

    /// <summary>
    /// Handles paginated service response
    /// </summary>
    public static IActionResult HandlePagedServiceResponse<T>(
        this ControllerBase controller,
        PagedResponse<T> response
    )
    {
        return response.ResponseStatusCode switch
        {
            ResponseStatusCode.Success => controller.Ok(
                new PagedDataResponse<T>(
                    response.Data,
                    response.Message ?? "Operation completed successfully",
                    new PaginationMetadata(
                        response.PageIndex,
                        response.PageSize,
                        response.TotalCount,
                        response.TotalPages,
                        response.HasPreviousPage,
                        response.HasNextPage
                    )
                )
            ),
            ResponseStatusCode.NoContent => controller.NoContent(),
            ResponseStatusCode.BadRequest => controller.BadRequest(
                new BadRequestResponse(response.Message ?? "Bad request")
            ),
            ResponseStatusCode.NotFound => controller.NotFound(
                new NotFoundResponse(response.Message ?? "Resource not found")
            ),
            ResponseStatusCode.Unauthorized => controller.Unauthorized(
                new ErrorResponse(response.Message ?? "Unauthorized access")
            ),
            ResponseStatusCode.Forbidden => controller.StatusCode(
                403,
                new ErrorResponse(response.Message ?? "Forbidden")
            ),
            ResponseStatusCode.Conflict => controller.Conflict(
                new ConflictResponse(response.Message ?? "Conflict occurred")
            ),
            ResponseStatusCode.ValidationError => controller.UnprocessableEntity(
                new ValidationErrorResponse(
                    response.Message ?? "Validation failed",
                    (response.ValidationErrors ?? [])
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                )
            ),
            ResponseStatusCode.Error or ResponseStatusCode.InternalServerError =>
                controller.StatusCode(
                    500,
                    new ServerErrorResponse(
                        response.Message ?? "An internal server error occurred",
                        response.Errors
                    )
                ),
            _ => controller.StatusCode(
                500,
                new ServerErrorResponse("An unexpected error occurred", null)
            ),
        };
    }
}