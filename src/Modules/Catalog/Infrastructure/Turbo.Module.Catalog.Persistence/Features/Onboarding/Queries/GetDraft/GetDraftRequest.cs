using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Queries.GetDraft;

public sealed record GetDraftRequest(Guid DraftId) : IQuery<AppConc.Response<GetDraftResponse>>;