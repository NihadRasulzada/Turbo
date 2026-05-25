namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenResponse(
    string AccessToken,
    string NewRefreshToken
);
