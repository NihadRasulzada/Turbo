using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetBrandById;

public sealed record GetBrandByIdRequest(Guid Id) : IQuery<AppConc.Response<GetBrandByIdResponse>>;
