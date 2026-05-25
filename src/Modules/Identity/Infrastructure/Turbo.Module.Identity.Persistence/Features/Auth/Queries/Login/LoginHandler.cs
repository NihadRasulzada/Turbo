using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Persistence.Context;
using RefreshTokenEntity = Turbo.Module.Identity.Domain.Entity.RefreshToken;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.Login;

public sealed class LoginHandler(
    IdentityQueryContext queryDb,
    IdentityCommandContext commandDb,
    IPasswordHasher passwordHasher,
    IJwtService jwtService
) : IRequestHandler<LoginQuery, LoginResponse>
{
    private const int MaxFailedAttempts = 5;
    private const int BlockDurationSeconds = 900; // 15 dəqiqə

    public async Task<LoginResponse> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();

        var user = await queryDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken)
            ?? throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new InvalidCredentialsException();

        if (user.IsCurrentlyBlocked())
            throw new UserBlockedException();

        var trackedUser = await commandDb.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken)
            ?? throw new InvalidCredentialsException();

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            trackedUser.RecordFailedLogin();

            if (trackedUser.FailedLoginCount >= MaxFailedAttempts)
                trackedUser.Block(BlockDurationSeconds);

            await commandDb.SaveChangesAsync(cancellationToken);
            throw new InvalidCredentialsException();
        }

        trackedUser.ResetFailedLogin();

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshTokenValue = jwtService.GenerateRefreshToken();
        var refreshToken = new RefreshTokenEntity(user.Id, refreshTokenValue);

        commandDb.RefreshTokens.Add(refreshToken);
        await commandDb.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshTokenValue, user.Id);
    }
}
