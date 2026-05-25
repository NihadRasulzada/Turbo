using FluentValidation;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

public sealed class SubmitDraftDetailsValidator : AbstractValidator<SubmitDraftDetailsRequest>
{
    public SubmitDraftDetailsValidator()
    {
        RuleFor(x => x.BrandId)
            .NotEqual(Guid.Empty).WithMessage("BrandId is required.");

        RuleFor(x => x.ModelId)
            .NotEqual(Guid.Empty).WithMessage("ModelId is required.");

        RuleFor(x => x.Year)
            .InclusiveBetween((short)1886, (short)DateTime.UtcNow.Year)
            .WithMessage($"Year must be between 1886 and {DateTime.UtcNow.Year}.");

        RuleFor(x => x.FuelType).IsInEnum().WithMessage("Invalid fuel type.");

        RuleFor(x => x.TransmissionType).IsInEnum().WithMessage("Invalid transmission type.");

        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");
    }
}