using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.DeleteModel;

public sealed class DeleteModelHandler(CommandDbContext db)
    : ICommandHandler<DeleteModelRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        DeleteModelRequest command, CancellationToken ct = default)
    {
        var model = await db.Models.FindAsync([command.Id], ct);
        if (model is null)
            return AppConc.Response.NotFound("Model not found.");

        var hasCars = await db.Cars.AnyAsync(c => c.ModelId == command.Id, ct);
        if (hasCars)
            return AppConc.Response.Conflict("Cannot delete model referenced by existing cars.");

        db.Models.Remove(model);
        await db.SaveChangesAsync(ct);
        return AppConc.Response.NoContent();
    }
}