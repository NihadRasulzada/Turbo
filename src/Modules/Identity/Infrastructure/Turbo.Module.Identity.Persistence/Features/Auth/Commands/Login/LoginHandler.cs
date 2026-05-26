using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using RefreshTokenEntity = Turbo.Module.Identity.Domain.Entity.RefreshToken;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed class LoginHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb,
    IPasswordHasher passwordHasher,
    IJwtService jwtService
) : ICommandHandler<LoginRequest, AppConc.Response<LoginResponse>>
{
    private const int MaxFailedAttempts = 5;
    private const int BlockDurationSeconds = 900; // 15 dəqiqə

    public async Task<AppConc.Response<LoginResponse>> HandleAsync(
        LoginRequest command, CancellationToken ct = default)
    {
        var normalizedEmail = command.Email.ToUpperInvariant();

        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, ct);

        if (user is null || !user.IsActive)
            return AppConc.Response<LoginResponse>.Unauthorized("Email or password is incorrect.");

        if (user.IsCurrentlyBlocked())
            return AppConc.Response<LoginResponse>.Forbidden("User is blocked. Please contact support.");

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            // Uğursuz cəhdi yaz; hər halda yeni Attach-dan istifadə et
            writeDb.Attach(user);
            user.RecordFailedLogin();
            if (user.FailedLoginCount >= MaxFailedAttempts)
                user.Block(BlockDurationSeconds);
            await writeDb.SaveChangesAsync(ct);

            return AppConc.Response<LoginResponse>.Unauthorized("Email or password is incorrect.");
        }

        // Uğurlu giriş: sayğacı sıfırla, bitmiş blok varsa bayrağı da təmizlə
        writeDb.Attach(user);
        user.OnSuccessfulLogin();

        // Access token və onun bitim tarixi eyni anda hesablanır
        var accessToken = jwtService.GenerateAccessToken(user);
        var accessTokenExpiresAt = jwtService.GetAccessTokenExpiresAt();

        // Raw token client-ə göndərilir; DB-də yalnız SHA-256 hash saxlanılır.
        // Belə ki, DB sızması bütün aktiv sessiyaları ifşa etmir.
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var hashedToken     = jwtService.HashRefreshToken(rawRefreshToken);
        var refreshToken    = new RefreshTokenEntity(user.Id, hashedToken);
        var refreshTokenExpiresAt =
            DateTimeOffset.FromUnixTimeSeconds(refreshToken.ExpiresAtSeconds).UtcDateTime;

        writeDb.Add(refreshToken);
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response<LoginResponse>.Success(
            new LoginResponse(accessToken, accessTokenExpiresAt, rawRefreshToken, refreshTokenExpiresAt, user.Id));
    }
}
