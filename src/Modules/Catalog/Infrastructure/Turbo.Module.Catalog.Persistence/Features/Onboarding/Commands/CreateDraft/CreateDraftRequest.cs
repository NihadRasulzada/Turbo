using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.CreateDraft;

public sealed record CreateDraftRequest : ICommand<AppConc.Response<CreateDraftResponse>>;
