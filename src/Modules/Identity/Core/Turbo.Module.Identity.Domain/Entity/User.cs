using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Identity.Domain.Entity;

public class User : BaseEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string NormalizedUserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsBlocked { get; private set; }
    public int FailedLoginCount { get; private set; }
    public long? BlockedUntilSeconds { get; private set; }

    private User(Guid id) : base(id)
    {
    }

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return new User(Guid.NewGuid())
        {
            Email = email,
            NormalizedEmail = normalizedEmail,
            UserName = email,
            NormalizedUserName = normalizedEmail,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            IsBlocked = false,
            FailedLoginCount = 0
        };
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Deactivate() => IsActive = false;

    public void Block(int durationSeconds)
    {
        IsBlocked = true;
        BlockedUntilSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + durationSeconds;
    }

    public void Unblock()
    {
        IsBlocked = false;
        BlockedUntilSeconds = null;
        FailedLoginCount = 0;
    }

    public void RecordFailedLogin()
    {
        FailedLoginCount++;
    }

    public void ResetFailedLogin()
    {
        FailedLoginCount = 0;
    }

    public bool IsCurrentlyBlocked()
    {
        if (!IsBlocked) return false;
        if (BlockedUntilSeconds is null) return true;
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() < BlockedUntilSeconds;
    }
}