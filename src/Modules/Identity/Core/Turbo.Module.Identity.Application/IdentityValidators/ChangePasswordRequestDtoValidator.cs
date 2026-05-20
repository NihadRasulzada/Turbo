using FluentValidation;
using Turbo.Module.Identity.Application.IdentityDTOs;

namespace Turbo.Module.Identity.Application.IdentityValidators;

public class ChangePasswordRequestDtoValidator
    : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required")
            .MinimumLength(6).WithMessage("Current password must be at least 6 characters");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters")
            .MaximumLength(64).WithMessage("New password must not exceed 64 characters")
            .Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter")
            .Matches(@"\d").WithMessage("New password must contain at least one digit")
            .Matches(@"[\W_]").WithMessage("New password must contain at least one special character")
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("New password cannot be the same as the current password");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match");
    }
}
