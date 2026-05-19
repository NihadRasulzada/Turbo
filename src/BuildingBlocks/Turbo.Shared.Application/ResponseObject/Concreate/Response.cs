using Turbo.Shared.Application.ResponseObject.Abstraction;
using Turbo.Shared.Application.ResponseObject.Enums;

namespace Turbo.Shared.Application.ResponseObject.Concreate;

public class Response : IResponse
{
    public string Message { get; set; }
    public ResponseStatusCode ResponseStatusCode { get; set; }
    public IEnumerable<CustomValidationError> ValidationErrors { get; set; }
    public IEnumerable<CustomError> Errors { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = [];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

#if DEBUG
    public string ExceptionMessage { get; set; }
    public string StackTrace { get; set; }
#endif

    public bool IsSuccess =>
        ResponseStatusCode == ResponseStatusCode.Success
        || ResponseStatusCode == ResponseStatusCode.Created;
    public bool IsFailure => !IsSuccess;

    public int HttpStatusCode =>
        ResponseStatusCode switch
        {
            ResponseStatusCode.Success => 200,
            ResponseStatusCode.Created => 201,
            ResponseStatusCode.NotFound => 404,
            ResponseStatusCode.BadRequest => 400,
            ResponseStatusCode.ValidationError => 422,
            ResponseStatusCode.Unauthorized => 401,
            ResponseStatusCode.Forbidden => 403,
            ResponseStatusCode.Conflict => 409,
            ResponseStatusCode.Error => 500,
            ResponseStatusCode.InternalServerError => 500,
            ResponseStatusCode.NoContent => 204,
            _ => 500,
        };

    public Response(ResponseStatusCode responseType)
    {
        ResponseStatusCode = responseType;
    }

    public Response(Response response)
    {
        Message = response.Message;
        ResponseStatusCode = response.ResponseStatusCode;
        ValidationErrors = response.ValidationErrors;
        Errors = response.Errors;
        Metadata = response.Metadata;
        Timestamp = response.Timestamp;

#if DEBUG
        ExceptionMessage = response.ExceptionMessage;
        StackTrace = response.StackTrace;
#endif
    }

    public Response(ResponseStatusCode responseType, IEnumerable<CustomError> errors)
        : this(responseType)
    {
        Errors = errors;
    }

    public Response(ResponseStatusCode responseType, string message)
        : this(responseType)
    {
        Message = message;
    }

    // Fluent API methods
    public Response WithMessage(string message)
    {
        Message = message;
        return this;
    }

    public Response WithMetadata(string key, object value)
    {
        Metadata[key] = value;
        return this;
    }

    public Response WithMetadata(Dictionary<string, object> metadata)
    {
        Metadata = metadata;
        return this;
    }

#if DEBUG
    public Response WithException(Exception ex)
    {
        ExceptionMessage = ex.Message;
        StackTrace = ex.StackTrace;
        return this;
    }
#endif

    // Static factory methods
    public static Response Success(string message = "Operation completed successfully") =>
        new(ResponseStatusCode.Success, message);

    public static Response Created(string message = "Resource created successfully") =>
        new(ResponseStatusCode.Created, message);

    public static Response BadRequest(string message) =>
        new(ResponseStatusCode.BadRequest, message);

    public static Response NotFound(string message) => new(ResponseStatusCode.NotFound, message);

    public static Response Unauthorized(string message = "Unauthorized access") =>
        new(ResponseStatusCode.Unauthorized, message);

    public static Response Forbidden(string message = "Forbidden") =>
        new(ResponseStatusCode.Forbidden, message);

    public static Response Conflict(string message) => new(ResponseStatusCode.Conflict, message);

    public static Response ValidationError(IEnumerable<CustomValidationError> errors) =>
        new(ResponseStatusCode.ValidationError) { ValidationErrors = errors };

    public static Response Error(string message) => new(ResponseStatusCode.Error, message);

    public static Response Error(IEnumerable<CustomError> errors) =>
        new(ResponseStatusCode.Error, errors);

    public static Response InternalServerError(string message = "Internal server error") =>
        new(ResponseStatusCode.InternalServerError, message);

    public static Response NoContent(string message = "No content") =>
        new(ResponseStatusCode.NoContent, message);

#if DEBUG
    public static Response Error(Exception ex) =>
        new Response(ResponseStatusCode.Error, ex.Message)
        {
            ExceptionMessage = ex.Message,
            StackTrace = ex.StackTrace,
        };
#endif
}
