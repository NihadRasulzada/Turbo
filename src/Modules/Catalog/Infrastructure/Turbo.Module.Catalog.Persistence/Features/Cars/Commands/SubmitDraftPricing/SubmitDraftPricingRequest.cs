using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftPricing;

public sealed record SubmitDraftPricingRequest(
    Guid DraftId,
    int Price,
    string Description
) : ICommand<AppConc.Response<SubmitDraftPricingResponse>>;