using FluentValidation;
using Turbo.Module.Catalog.Domain.Extensions;

namespace Turbo.Module.Catalog.Persistence.Features.Car.Commands.Add;

public sealed class AddCarValidator : AbstractValidator<AddCarRequest>
{
    public AddCarValidator()
    {
        RuleFor(x => x.Brand).IsInEnum().WithMessage("Invalid brand.");

        RuleFor(x => x.Model)
            .IsInEnum()
            .WithMessage("Invalid model.")
            .Must((req, model) => model.BelongsTo(req.Brand))
            .WithMessage(req => $"Model does not belong to brand {req.Brand}.");

        RuleFor(x => x.Year)
            .InclusiveBetween((short)1886, (short)DateTime.UtcNow.Year)
            .WithMessage($"Year must be between 1886 and {DateTime.UtcNow.Year}.");

        RuleFor(x => x.FuelType).IsInEnum().WithMessage("Invalid fuel type.");

        RuleFor(x => x.TransmissionType).IsInEnum().WithMessage("Invalid transmission type.");

        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");

        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Images).NotEmpty().WithMessage("At least one image is required.");
    }
}
