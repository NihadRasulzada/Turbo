using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Model.Commands.DeleteModel;

public sealed record DeleteModelRequest(Guid Id) : ICommand<AppConc.Response>;