using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.CreateModel;

public sealed record CreateModelRequest(string Name, Guid BrandId) : ICommand<AppConc.Response<ModelResponse>>;