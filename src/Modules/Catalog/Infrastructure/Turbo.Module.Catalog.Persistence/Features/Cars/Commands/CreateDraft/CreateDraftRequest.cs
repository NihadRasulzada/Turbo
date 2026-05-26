using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.CreateDraft;

public sealed record CreateDraftRequest(Guid SellerId) : ICommand<AppConc.Response<CreateDraftResponse>>;
