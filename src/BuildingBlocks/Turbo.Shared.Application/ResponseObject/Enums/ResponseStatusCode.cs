namespace Turbo.Shared.Application.ResponseObject.Enums;

public enum ResponseStatusCode
{
    Success = 1,
    Created = 2,
    ValidationError = 3,
    NotFound = 4,
    Error = 5,
    BadRequest = 6,
    Unauthorized = 7,
    Forbidden = 8,
    Conflict = 9,
    InternalServerError = 10,
    NoContent = 11,
}