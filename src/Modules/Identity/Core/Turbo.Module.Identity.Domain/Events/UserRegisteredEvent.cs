namespace Turbo.Module.Identity.Domain.Events;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime RegisteredAt
);
