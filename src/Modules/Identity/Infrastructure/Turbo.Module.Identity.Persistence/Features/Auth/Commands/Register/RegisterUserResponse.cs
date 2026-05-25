namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed record RegisterUserResponse(Guid UserId, string Email);
