using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Queries.GetModelById;

public sealed record GetModelByIdRequest(Guid Id) : IQuery<AppConc.Response<GetModelByIdResponse>>;
