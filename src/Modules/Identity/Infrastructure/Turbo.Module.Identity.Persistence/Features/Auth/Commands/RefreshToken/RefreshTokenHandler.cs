using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using RefreshTokenEntity = Turbo.Module.Identity.Domain.Entity.RefreshToken;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb,
    IJwtService jwtService
) : ICommandHandler<RefreshTokenRequest, AppConc.Response<RefreshTokenResponse>>
{
    public async Task<AppConc.Response<RefreshTokenResponse>> HandleAsync(
        RefreshTokenRequest command, CancellationToken ct = default)
    {
        // Client raw token göndərir; DB-də hash var → gələni hash-ləyib axtarırıq.
        var hashedInput = jwtService.HashRefreshToken(command.RefreshToken);

        var existing = await readDb.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == hashedInput, ct);

        if (existing is null || !existing.IsValid())
            return AppConc.Response<RefreshTokenResponse>.Unauthorized(
                "Refresh token is invalid or expired.");

        // Bloklanmış və ya deaktiv user yeni token ala bilməz
        if (!existing.User.IsActive)
            return AppConc.Response<RefreshTokenResponse>.Unauthorized(
                "Refresh token is invalid or expired.");

        if (existing.User.IsCurrentlyBlocked())
            return AppConc.Response<RefreshTokenResponse>.Forbidden(
                "User is blocked. Please contact support.");

        // Köhnə tokeni revoke et; User navigation-u ayrıca Attach etmə
        // (Include ilə yüklənmiş qraf writeDb-yə birlikdə gəlir)
        writeDb.Attach(existing);
        existing.Revoke();

        // Yeni raw token client-ə göndərilir; DB-də hash saxlanılır.
        var newRawToken    = jwtService.GenerateRefreshToken();
        var newHashedToken = jwtService.HashRefreshToken(newRawToken);
        var newToken       = new RefreshTokenEntity(existing.UserId, newHashedToken);
        var refreshTokenExpiresAt =
            DateTimeOffset.FromUnixTimeSeconds(newToken.ExpiresAtSeconds).UtcDateTime;
        writeDb.Add(newToken);

        await writeDb.SaveChangesAsync(ct);

        // Access token və onun bitim tarixi eyni anda hesablanır
        var accessToken = jwtService.GenerateAccessToken(existing.User);
        var accessTokenExpiresAt = jwtService.GetAccessTokenExpiresAt();

        return AppConc.Response<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(accessToken, accessTokenExpiresAt, newRawToken, refreshTokenExpiresAt));
    }
}
