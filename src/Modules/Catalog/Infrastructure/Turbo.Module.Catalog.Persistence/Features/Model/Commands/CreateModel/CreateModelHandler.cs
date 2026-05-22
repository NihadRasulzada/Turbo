using FluentValidation;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainModel = Turbo.Module.Catalog.Domain.Entity.Model;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;

public sealed class CreateModelHandler(CommandDbContext db, IValidator<CreateModelRequest> validator)
    : ICommandHandler<CreateModelRequest, AppConc.Response<ModelResponse>>
{
    public async Task<AppConc.Response<ModelResponse>> HandleAsync(
        CreateModelRequest command, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return AppConc.Response<ModelResponse>.ValidationError(
                validation.Errors.Select(e => new CustomValidationError(e.PropertyName, e.ErrorMessage)));

        var brandExists = await db.Brands.FindAsync([command.BrandId], ct) is not null;
        if (!brandExists)
            return AppConc.Response<ModelResponse>.NotFound("Brand not found.");

        var model = new DomainModel(command.Name, command.BrandId);
        await db.Models.AddAsync(model, ct);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<ModelResponse>.Created(new ModelResponse(model.Id, model.Name, model.BrandId));
    }
}
