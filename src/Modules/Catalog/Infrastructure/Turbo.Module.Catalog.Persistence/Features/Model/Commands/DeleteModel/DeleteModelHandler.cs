using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;
using DomainModel = Turbo.Module.Catalog.Domain.Entity.Model;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.DeleteModel;

public sealed class DeleteModelHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<DeleteModelRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        DeleteModelRequest command, CancellationToken ct = default)
    {
        var model = await readDb.Models
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == command.Id, ct);
        if (model is null)
            return AppConc.Response.NotFound("Model not found.");

        var hasCars = await readDb.Cars.AnyAsync(c => c.ModelId == command.Id, ct);
        if (hasCars)
            return AppConc.Response.Conflict("Cannot delete model referenced by existing cars.");

        writeDb.Remove(model);
        await writeDb.SaveChangesAsync(ct);
        return AppConc.Response.NoContent();
    }
}