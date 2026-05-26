using Turbo.Module.Identity.Domain.Entity;

namespace Turbo.Module.Identity.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();

    /// <summary>
    /// Access tokenin UTC bitim tarixini qaytarır.
    /// <see cref="GenerateAccessToken"/> ilə eyni anda çağırılmalıdır ki,
    /// token içindəki <c>exp</c> iddiası ilə sinxron qalsın.
    /// </summary>
    DateTime GetAccessTokenExpiresAt();

    /// <summary>
    /// Refresh tokenin SHA-256 hashini qaytarır.
    /// DB-də yalnız hash saxlanılır; client raw token alır.
    /// Lookup zamanı gələn raw token hash-lənib müqayisə edilir.
    /// </summary>
    string HashRefreshToken(string rawToken);
}