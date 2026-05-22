using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.UpdateModel;

public sealed record UpdateModelRequest(Guid Id, string Name, Guid BrandId)
    : ICommand<AppConc.Response<ModelResponse>>;