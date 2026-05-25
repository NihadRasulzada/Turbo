namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string NewRefreshToken,
    DateTime RefreshTokenExpiresAt
);
