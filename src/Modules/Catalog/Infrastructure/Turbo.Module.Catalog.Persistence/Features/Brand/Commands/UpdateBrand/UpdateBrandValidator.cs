using FluentValidation;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed class UpdateBrandValidator : AbstractValidator<UpdateBrandRequest>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(100).WithMessage("Brand name must not exceed 100 characters.");
    }
}
