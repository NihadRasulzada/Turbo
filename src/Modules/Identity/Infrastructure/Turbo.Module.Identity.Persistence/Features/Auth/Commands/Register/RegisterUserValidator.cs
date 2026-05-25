using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    private const int MinPasswordLength = 8;
    private const int MaxPasswordLength = 128;
    private const int MaxNameLength = 100;
    private const int MaxEmailLength = 256;

    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
            .MaximumLength(MaxEmailLength)
                .WithMessage($"Email must not exceed {MaxEmailLength} characters.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.")
            .MinimumLength(MinPasswordLength)
                .WithMessage($"Password must be at least {MinPasswordLength} characters.")
            .MaximumLength(MaxPasswordLength)
                .WithMessage($"Password must not exceed {MaxPasswordLength} characters.")
            .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]")
                .WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
                .WithMessage("First name is required.")
            .MaximumLength(MaxNameLength)
                .WithMessage($"First name must not exceed {MaxNameLength} characters.")
            .Matches(@"^[\p{L}\s\-']+$")
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.LastName)
            .NotEmpty()
                .WithMessage("Last name is required.")
            .MaximumLength(MaxNameLength)
                .WithMessage($"Last name must not exceed {MaxNameLength} characters.")
            .Matches(@"^[\p{L}\s\-']+$")
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");
    }
}