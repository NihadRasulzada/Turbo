namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;

public sealed record CreateModelResponse(Guid Id, string Name, Guid BrandId);
