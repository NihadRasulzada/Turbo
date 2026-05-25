using Microsoft.EntityFrameworkCore;
using Turbo.Module.Catalog.Domain.Entity;
using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Module.Catalog.Persistence.Contexts;

using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

public sealed class SubmitDraftDetailsHandler(
    ICatalogWriteDbContext writeDb,
    ICatalogReadDbContext readDb)
    : ICommandHandler<SubmitDraftDetailsRequest, AppConc.Response<SubmitDraftDetailsResponse>>
{
    public async Task<AppConc.Response<SubmitDraftDetailsResponse>> HandleAsync(
        SubmitDraftDetailsRequest command,
        CancellationToken ct = default)
    {
        var draft = await readDb.CarDrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == command.DraftId, ct);
        if (draft is null)
            return AppConc.Response<SubmitDraftDetailsResponse>.NotFound("Draft not found.");
        if (draft.Status == CarDraftStatus.Completed)
            return AppConc.Response<SubmitDraftDetailsResponse>.BadRequest("Draft is already completed.");
        if (draft.CurrentStep != 2)
            return AppConc.Response<SubmitDraftDetailsResponse>.BadRequest(
                draft.CurrentStep < 2
                    ? "Complete the images step first."
                    : "Details step has already been submitted.");

        var brandExists = await readDb.Brands.AnyAsync(b => b.Id == command.BrandId, ct);
        if (!brandExists)
            return AppConc.Response<SubmitDraftDetailsResponse>.NotFound("Brand not found.");

        var modelExists = await readDb.Models
            .AnyAsync(m => m.Id == command.ModelId && m.BrandId == command.BrandId, ct);
        if (!modelExists)
            return AppConc.Response<SubmitDraftDetailsResponse>.NotFound(
                "Model not found or does not belong to the specified brand.");

        writeDb.Attach(draft);
        draft.SetDetails(
            command.BrandId, command.ModelId, command.Year,
            command.FuelType, command.TransmissionType, command.Mileage);
        draft.AdvanceStep();
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response<SubmitDraftDetailsResponse>.Success(
            new SubmitDraftDetailsResponse(command.DraftId, 2, 3, "pricing"));
    }
}
