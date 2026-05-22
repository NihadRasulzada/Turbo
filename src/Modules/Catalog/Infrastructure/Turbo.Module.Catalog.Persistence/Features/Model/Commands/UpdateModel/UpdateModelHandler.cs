using FluentValidation;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.ResponseObject;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed class UpdateModelHandler(CommandDbContext db, IValidator<UpdateModelRequest> validator)
    : ICommandHandler<UpdateModelRequest, AppConc.Response<ModelResponse>>
{
    public async Task<AppConc.Response<ModelResponse>> HandleAsync(
        UpdateModelRequest command, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return AppConc.Response<ModelResponse>.ValidationError(
                validation.Errors.Select(e => new CustomValidationError(e.PropertyName, e.ErrorMessage)));

        var model = await db.Models.FindAsync([command.Id], ct);
        if (model is null)
            return AppConc.Response<ModelResponse>.NotFound("Model not found.");

        var brandExists = await db.Brands.FindAsync([command.BrandId], ct) is not null;
        if (!brandExists)
            return AppConc.Response<ModelResponse>.NotFound("Brand not found.");

        model.Update(command.Name, command.BrandId);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<ModelResponse>.Success(new ModelResponse(model.Id, model.Name, model.BrandId));
    }
}