using FluentValidation;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;

public sealed class BlockUserValidator : AbstractValidator<BlockUserRequest>
{
    private const int MaxDurationSeconds = 30 * 24 * 3600; // 30 gün

    public BlockUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
                .WithMessage("Block duration must be greater than zero.")
            .LessThanOrEqualTo(MaxDurationSeconds)
                .WithMessage($"Block duration cannot exceed {MaxDurationSeconds} seconds (30 days).");
    }
}
