using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Persistence.Context;
using RefreshTokenEntity = Turbo.Module.Identity.Domain.Entity.RefreshToken;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.RefreshToken;

public sealed class RefreshTokenHandler(
    IdentityQueryContext queryDb,
    IdentityCommandContext commandDb,
    IJwtService jwtService
) : IRequestHandler<RefreshTokenQuery, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(
        RefreshTokenQuery request,
        CancellationToken cancellationToken)
    {
        var existing = await queryDb.RefreshTokens
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        if (!existing.IsValid())
            throw new InvalidRefreshTokenException();

        var trackedToken = await commandDb.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == existing.Id, cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        trackedToken.Revoke();

        var newTokenValue = jwtService.GenerateRefreshToken();
        var newToken = new RefreshTokenEntity(existing.UserId, newTokenValue);

        commandDb.RefreshTokens.Add(newToken);
        await commandDb.SaveChangesAsync(cancellationToken);

        var accessToken = jwtService.GenerateAccessToken(existing.User);
        return new RefreshTokenResponse(accessToken, newTokenValue);
    }
}