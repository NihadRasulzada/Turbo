using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed class UpdateModelHandler(CommandDbContext db)
    : ICommandHandler<UpdateModelRequest, AppConc.Response<UpdateModelResponse>>
{
    public async Task<AppConc.Response<UpdateModelResponse>> HandleAsync(
        UpdateModelRequest command, CancellationToken ct = default)
    {
        var model = await db.Models.FindAsync([command.Id], ct);
        if (model is null)
            return AppConc.Response<UpdateModelResponse>.NotFound("Model not found.");

        var brandExists = await db.Brands.FindAsync([command.BrandId], ct) is not null;
        if (!brandExists)
            return AppConc.Response<UpdateModelResponse>.NotFound("Brand not found.");

        model.Update(command.Name, command.BrandId);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<UpdateModelResponse>.Success(
            new UpdateModelResponse(model.Id, model.Name, model.BrandId));
    }
}
