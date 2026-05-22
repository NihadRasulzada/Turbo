using Turbo.Shared.Application.ResponseObject.Enums;

namespace Turbo.Shared.Application.ResponseObject.Abstraction;

public interface IResponse
{
    string Message { get; set; }
    ResponseStatusCode ResponseStatusCode { get; set; }
    IEnumerable<CustomValidationError> ValidationErrors { get; set; }
    IEnumerable<CustomError> Errors { get; set; }
    Dictionary<string, object> Metadata { get; set; }
    DateTime Timestamp { get; set; }
    bool IsSuccess { get; }
    bool IsFailure { get; }
    int HttpStatusCode { get; }

#if DEBUG
    string ExceptionMessage { get; set; }
    string StackTrace { get; set; }
#endif
}