using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Shared.Infrastructure.EntityConfiguration;

namespace Turbo.Module.Identity.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(rt => rt.Token).IsUnique();

        builder.Property(rt => rt.UserId).IsRequired();

        builder.Property(rt => rt.ExpiresAtSeconds).IsRequired();
    }
}
