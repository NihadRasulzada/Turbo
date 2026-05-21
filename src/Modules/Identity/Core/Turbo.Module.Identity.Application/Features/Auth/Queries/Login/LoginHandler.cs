using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Exceptions;

namespace Turbo.Module.Identity.Application.Features.Auth.Queries.Login;

public class LoginHandler(
    IWriteDbContext writeDb,
    IPasswordHasher passwordHasher,
    IJwtService jwtService
) : IRequestHandler<LoginQuery, LoginResult>
{
    public async Task<LoginResult> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await writeDb.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken)
            ?? throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new InvalidCredentialsException();

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshTokenValue = jwtService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue);

        writeDb.RefreshTokens.Add(refreshToken);
        await writeDb.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken, refreshTokenValue, user.Id);
    }
}
