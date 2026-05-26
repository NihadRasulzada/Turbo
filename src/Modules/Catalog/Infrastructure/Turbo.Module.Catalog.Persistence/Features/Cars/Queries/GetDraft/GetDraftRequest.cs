using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetDraft;

public sealed record GetDraftRequest(Guid DraftId, Guid RequesterId) : IQuery<AppConc.Response<GetDraftResponse>>;
