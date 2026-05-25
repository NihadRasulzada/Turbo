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
        var existing = await readDb.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, ct);

        if (existing is null || !existing.IsValid())
            return AppConc.Response<RefreshTokenResponse>.Unauthorized(
                "Refresh token is invalid or expired.");

        writeDb.Attach(existing);
        existing.Revoke();

        var newTokenValue = jwtService.GenerateRefreshToken();
        var newToken = new RefreshTokenEntity(existing.UserId, newTokenValue);
        writeDb.Add(newToken);

        await writeDb.SaveChangesAsync(ct);

        var accessToken = jwtService.GenerateAccessToken(existing.User);
        return AppConc.Response<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(accessToken, newTokenValue));
    }
}
