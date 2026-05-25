using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.Login;

public sealed class LoginValidator : AbstractValidator<LoginQuery>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
            .MaximumLength(256)
                .WithMessage("Email must not exceed 256 characters.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.")
            .MinimumLength(1)
                .WithMessage("Password must not be empty.")
            .MaximumLength(128)
                .WithMessage("Password must not exceed 128 characters.");
    }
}
