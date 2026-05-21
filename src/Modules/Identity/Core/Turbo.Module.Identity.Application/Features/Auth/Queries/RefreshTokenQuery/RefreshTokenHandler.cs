using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Application.Features.Auth.Queries.RefreshTokenQuery;

public class RefreshTokenHandler(
    IWriteDbContext writeDb,
    IJwtService jwtService
) : IRequestHandler<RefreshTokenQuery, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(
        RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var existing = await writeDb.RefreshTokens
            .Include(rt => rt.User)   // navigation property əlavə edin
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        if (!existing.IsValid())
            throw new InvalidRefreshTokenException();

        existing.Revoke();

        var newTokenValue = jwtService.GenerateRefreshToken();
        var newToken = RefreshToken.Create(existing.UserId, newTokenValue);

        writeDb.RefreshTokens.Add(newToken);
        await writeDb.SaveChangesAsync(cancellationToken);

        var accessToken = jwtService.GenerateAccessToken(existing.User);
        return new RefreshTokenResult(accessToken, newTokenValue);
    }
}
