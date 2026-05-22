using FluentValidation;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed class UpdateBrandHandler(CommandDbContext db, IValidator<UpdateBrandRequest> validator)
    : ICommandHandler<UpdateBrandRequest, AppConc.Response<BrandResponse>>
{
    public async Task<AppConc.Response<BrandResponse>> HandleAsync(
        UpdateBrandRequest command, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return AppConc.Response<BrandResponse>.ValidationError(
                validation.Errors.Select(e => new CustomValidationError(e.PropertyName, e.ErrorMessage)));

        var brand = await db.Brands.FindAsync([command.Id], ct);
        if (brand is null)
            return AppConc.Response<BrandResponse>.NotFound("Brand not found.");

        brand.UpdateName(command.Name);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<BrandResponse>.Success(new BrandResponse(brand.Id, brand.Name));
    }
}
