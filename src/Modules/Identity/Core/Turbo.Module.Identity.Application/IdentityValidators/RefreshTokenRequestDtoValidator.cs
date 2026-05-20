using FluentValidation;
using Turbo.Module.Identity.Application.IdentityDTOs;

namespace Turbo.Module.Identity.Application.IdentityValidators;

public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required")
            .MinimumLength(10).WithMessage("Invalid refresh token");
    }
}
