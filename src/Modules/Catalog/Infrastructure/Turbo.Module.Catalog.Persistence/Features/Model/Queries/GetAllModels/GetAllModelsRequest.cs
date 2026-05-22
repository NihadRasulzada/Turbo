using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetAllModels;

public sealed record GetAllModelsRequest(Guid? BrandId = null)
    : IQuery<AppConc.Response<IReadOnlyList<ModelResponse>>>;
