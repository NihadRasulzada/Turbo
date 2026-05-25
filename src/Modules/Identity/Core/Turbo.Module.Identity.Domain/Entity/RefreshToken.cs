using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Identity.Domain.Entity;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public long ExpiresAtSeconds { get; private set; }
    public bool IsRevoked { get; private set; }

    public User User { get; private set; } = null!;

    public RefreshToken(
        Guid userId,
        string token,
        int expiryDays = 7,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        UserId = userId;
        Token = token;
        ExpiresAtSeconds =
            DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (expiryDays * 86400L);

        IsRevoked = false;
    }


    private RefreshToken(Guid id) : base(id)
    {
    }

    public bool IsValid() =>
        !IsRevoked &&
        DateTimeOffset.UtcNow.ToUnixTimeSeconds() < ExpiresAtSeconds;

    public void Revoke() => IsRevoked = true;
}