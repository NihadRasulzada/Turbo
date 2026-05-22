using Turbo.Shared.Application.ResponseObject.Abstraction;
using Turbo.Shared.Application.ResponseObject.Enums;

namespace Turbo.Shared.Application.ResponseObject.Concreate;

public class Response<T> : Response, IResponse<T>
{
    public T Data { get; set; }

    public Response(ResponseStatusCode responseType, T data)
        : base(responseType)
    {
        Data = data;
    }

    public Response(Response response, T data)
        : base(response)
    {
        Data = data;
    }

    public Response(ResponseStatusCode responseType, string message)
        : base(responseType, message) { }

    public Response(ResponseStatusCode responseType, T data, string message)
        : base(responseType, message)
    {
        Data = data;
    }

    public Response(ResponseStatusCode responseType, T data, List<CustomValidationError> errors)
        : base(responseType)
    {
        Data = data;
        ValidationErrors = errors;
    }

    // Fluent API methods
    public new Response<T> WithMessage(string message)
    {
        Message = message;
        return this;
    }

    public new Response<T> WithMetadata(string key, object value)
    {
        Metadata[key] = value;
        return this;
    }

    public new Response<T> WithMetadata(Dictionary<string, object> metadata)
    {
        Metadata = metadata;
        return this;
    }

    public Response<T> WithData(T data)
    {
        Data = data;
        return this;
    }

#if DEBUG
    public new Response<T> WithException(Exception ex)
    {
        ExceptionMessage = ex.Message;
        StackTrace = ex.StackTrace;
        return this;
    }
#endif

    // Static factory methods
    public static Response<T> Success(
        T data,
        string message = "Operation completed successfully"
    ) => new(ResponseStatusCode.Success, data, message);

    public static Response<T> Success(T data, string message, Dictionary<string, object> metadata)
    {
        var response = new Response<T>(ResponseStatusCode.Success, data, message)
        {
            Metadata = metadata,
        };
        return response;
    }

    public static Response<T> Created(T data, string message = "Resource created successfully") =>
        new(ResponseStatusCode.Created, data, message);

    public static Response<T> Created(T data, string message, Dictionary<string, object> metadata)
    {
        var response = new Response<T>(ResponseStatusCode.Created, data, message)
        {
            Metadata = metadata,
        };
        return response;
    }

    public static new Response<T> BadRequest(string message) =>
        new(ResponseStatusCode.BadRequest, message);

    public static Response<T> BadRequest(string message, T data) =>
        new(ResponseStatusCode.BadRequest, data, message);

    public static new Response<T> NotFound(string message) =>
        new(ResponseStatusCode.NotFound, message);

    public static Response<T> NotFound(string message, T data) =>
        new(ResponseStatusCode.NotFound, data, message);

    public static new Response<T> Unauthorized(string message = "Unauthorized access") =>
        new(ResponseStatusCode.Unauthorized, message);

    public static new Response<T> Forbidden(string message = "Forbidden") =>
        new(ResponseStatusCode.Forbidden, message);

    public static new Response<T> Conflict(string message) =>
        new(ResponseStatusCode.Conflict, message);

    public static Response<T> Conflict(string message, T data) =>
        new(ResponseStatusCode.Conflict, data, message);

    public static new Response<T> ValidationError(IEnumerable<CustomValidationError> errors) =>
        new(ResponseStatusCode.ValidationError, default(T)) { ValidationErrors = errors };

    public static Response<T> ValidationError(IEnumerable<CustomValidationError> errors, T data) =>
        new(ResponseStatusCode.ValidationError, data) { ValidationErrors = errors };

    public static new Response<T> Error(string message) => new(ResponseStatusCode.Error, message);

    public static Response<T> Error(string message, T data) =>
        new(ResponseStatusCode.Error, data, message);

    public static new Response<T> InternalServerError(string message = "Internal server error") =>
        new(ResponseStatusCode.InternalServerError, message);

    public static new Response<T> NoContent(string message = "No content") =>
        new(ResponseStatusCode.NoContent, message);

#if DEBUG
    public static Response<T> Error(Exception ex) =>
        new Response<T>(ResponseStatusCode.Error, ex.Message)
        {
            ExceptionMessage = ex.Message,
            StackTrace = ex.StackTrace,
        };

    public static Response<T> Error(Exception ex, T data) =>
        new Response<T>(ResponseStatusCode.Error, data, ex.Message)
        {
            ExceptionMessage = ex.Message,
            StackTrace = ex.StackTrace,
        };
#endif
}