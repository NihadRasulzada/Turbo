using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainModel = Turbo.Module.Catalog.Domain.Entity.Model;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;

public sealed class CreateModelHandler(CommandDbContext db)
    : ICommandHandler<CreateModelRequest, AppConc.Response<CreateModelResponse>>
{
    public async Task<AppConc.Response<CreateModelResponse>> HandleAsync(
        CreateModelRequest command, CancellationToken ct = default)
    {
        var brandExists = await db.Brands.FindAsync([command.BrandId], ct) is not null;
        if (!brandExists)
            return AppConc.Response<CreateModelResponse>.NotFound("Brand not found.");

        var model = new DomainModel(command.Name, command.BrandId);
        await db.Models.AddAsync(model, ct);
        await db.SaveChangesAsync(ct);
        return AppConc.Response<CreateModelResponse>.Created(
            new CreateModelResponse(model.Id, model.Name, model.BrandId));
    }
}
