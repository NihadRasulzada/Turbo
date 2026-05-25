namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetAllModels;

public sealed record GetAllModelsResponse(Guid Id, string Name, Guid BrandId);