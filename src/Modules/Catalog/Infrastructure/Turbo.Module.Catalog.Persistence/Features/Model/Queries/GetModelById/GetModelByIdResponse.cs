namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;

public sealed record GetModelByIdResponse(Guid Id, string Name, Guid BrandId);
