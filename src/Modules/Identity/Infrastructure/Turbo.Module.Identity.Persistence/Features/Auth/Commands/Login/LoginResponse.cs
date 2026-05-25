namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    Guid UserId
);
