using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetCarConfig;

public sealed record GetCarConfigRequest : IQuery<AppConc.Response<GetCarConfigResponse>>;