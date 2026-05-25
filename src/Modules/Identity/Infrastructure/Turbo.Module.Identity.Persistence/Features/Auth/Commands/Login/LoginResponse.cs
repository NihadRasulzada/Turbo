namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    Guid UserId
);
