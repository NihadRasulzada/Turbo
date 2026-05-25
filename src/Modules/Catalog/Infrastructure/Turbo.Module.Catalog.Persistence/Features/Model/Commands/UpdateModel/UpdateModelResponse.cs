namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed record UpdateModelResponse(Guid Id, string Name, Guid BrandId);