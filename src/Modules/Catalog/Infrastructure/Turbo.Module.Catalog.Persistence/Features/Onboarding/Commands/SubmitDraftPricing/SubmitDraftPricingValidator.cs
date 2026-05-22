using FluentValidation;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftPricing;

public sealed class SubmitDraftPricingValidator : AbstractValidator<SubmitDraftPricingRequest>
{
    public SubmitDraftPricingValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
    }
}
