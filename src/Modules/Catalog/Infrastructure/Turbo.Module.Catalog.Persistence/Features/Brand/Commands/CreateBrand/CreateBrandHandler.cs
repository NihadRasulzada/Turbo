using FluentValidation;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainBrand = Turbo.Module.Catalog.Domain.Entity.Brand;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.CreateBrand;

public sealed class CreateBrandHandler(CommandDbContext db, IValidator<CreateBrandRequest> validator)
    : ICommandHandler<CreateBrandRequest, AppConc.Response<BrandResponse>>
{
    public async Task<AppConc.Response<BrandResponse>> HandleAsync(
        CreateBrandRequest command, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return AppConc.Response<BrandResponse>.ValidationError(
                validation.Errors.Select(e => new CustomValidationError(e.PropertyName, e.ErrorMessage)));

        var brand = new DomainBrand(command.Name);
        await db.Brands.AddAsync(brand, ct);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<BrandResponse>.Created(new BrandResponse(brand.Id, brand.Name));
    }
}