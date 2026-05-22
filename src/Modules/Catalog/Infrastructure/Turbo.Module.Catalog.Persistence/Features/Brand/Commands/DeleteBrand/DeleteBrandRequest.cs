using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Brand.Commands.DeleteBrand;

public sealed record DeleteBrandRequest(Guid Id) : ICommand<AppConc.Response>;