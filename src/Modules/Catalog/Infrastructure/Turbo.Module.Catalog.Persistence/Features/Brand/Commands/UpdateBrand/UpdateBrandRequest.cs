using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.UpdateBrand;

public sealed record UpdateBrandRequest(Guid Id, string Name) : ICommand<AppConc.Response<BrandResponse>>;
