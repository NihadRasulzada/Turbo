using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Queries.GetAllBrands;

public sealed record GetAllBrandsRequest : IQuery<AppConc.Response<IReadOnlyList<BrandResponse>>>;
