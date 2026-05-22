using FluentValidation;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;

public sealed class CreateBrandValidator : AbstractValidator<CreateBrandRequest>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(100).WithMessage("Brand name must not exceed 100 characters.");
    }
}