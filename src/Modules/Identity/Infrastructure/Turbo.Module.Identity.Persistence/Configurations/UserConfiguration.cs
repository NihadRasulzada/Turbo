using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Shared.Infrastructure.EntityConfiguration;

namespace Turbo.Module.Identity.Persistence.Configurations;

public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.NormalizedEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.NormalizedEmail).IsUnique();

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.NormalizedUserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.NormalizedUserName).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.IsBlocked).IsRequired();
        builder.Property(u => u.FailedLoginCount).IsRequired();

        builder.Property(u => u.BlockedUntilSeconds)
            .IsRequired(false);

        builder.HasMany<RefreshToken>()
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
