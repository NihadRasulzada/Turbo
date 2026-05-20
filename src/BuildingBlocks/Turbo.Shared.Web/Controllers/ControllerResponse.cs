using Turbo.Shared.Application.ResponseObject;

namespace Turbo.Shared.Web.Controllers;

/// <summary>
/// Standard success response without data
/// </summary>
public class SuccessResponse(string message)
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Success message describing the operation result
    /// </summary>
    /// <example>Entity created successfully</example>
    public string Message { get; set; } = message;
}

public class SuccessResponse<T>(T data, string message)
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    public T Data { get; set; } = data;

    /// <summary>
    /// Success message describing the operation result
    /// </summary>
    /// <example>Entity created successfully</example>
    public string Message { get; set; } = message;
}

public class CreatedResponse<T>(T data, string message) : SuccessResponse<T>(data, message) { }

/// <summary>
/// Error response for client errors (400, 404)
/// </summary>
public class ErrorResponse(string message)
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    /// <example>false</example>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Error message describing what went wrong
    /// </summary>
    /// <example>Entity with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found</example>
    public string Message { get; set; } = message;
}

/// <summary>
/// Validation error response (422)
/// </summary>
public class ValidationErrorResponse(string message, Dictionary<string, string[]> errors)
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    /// <example>false</example>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Error message
    /// </summary>
    /// <example>Validation failed</example>
    public string Message { get; set; } = message;

    /// <summary>
    /// Validation errors grouped by property name
    /// </summary>
    /// <example>{"name":["Name is required","Name must be unique"],"description":["Description must not exceed 500 characters"]}</example>
    public Dictionary<string, string[]> Errors { get; set; } = errors;
}

/// <summary>
/// Internal server error response (500)
/// </summary>
public class ServerErrorResponse(string message, IEnumerable<CustomError> errors)
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    /// <example>false</example>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Error message
    /// </summary>
    /// <example>An internal server error occurred</example>
    public string Message { get; set; } = message;

    /// <summary>
    /// Additional error details (optional)
    /// </summary>
    public IEnumerable<CustomError>? Errors { get; set; } = errors;
}

/// <summary>
/// Standard success response with data payload
/// </summary>
/// <typeparam name="T">Type of the data returned</typeparam>
public class PagedDataResponse<T>(T? data, string message, PaginationMetadata metadata)
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Success message describing the operation result
    /// </summary>
    /// <example>Operation completed successfully</example>
    public string Message { get; set; } = message;

    /// <summary>
    /// The returned data payload
    /// </summary>
    public T? Data { get; set; } = data;

    public PaginationMetadata PaginationMetadata { get; set; } = metadata;
}

public class NotFoundResponse(string message)
{
    public string Message { get; set; } = message;
}

public class ConflictResponse(string message)
{
    public string Message { get; set; } = message;
}

public class BadRequestResponse(string message)
{
    public string Message { get; set; } = message;
}

public class PaginationMetadata(
    int pageIndex,
    int pageSize,
    int totalCount,
    int totalPages,
    bool hasPreviousPage,
    bool hasNextPage
)
{
    public int PageIndex { get; set; } = pageIndex;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public int TotalPages { get; set; } = totalPages;
    public bool HasPreviousPage { get; set; } = hasPreviousPage;
    public bool HasNextPage { get; set; } = hasNextPage;
}