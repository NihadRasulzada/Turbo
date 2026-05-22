using FluentValidation;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed class UpdateModelValidator : AbstractValidator<UpdateModelRequest>
{
    public UpdateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Model name is required.")
            .MaximumLength(100).WithMessage("Model name must not exceed 100 characters.");

        RuleFor(x => x.BrandId)
            .NotEqual(Guid.Empty).WithMessage("BrandId is required.");
    }
}
