using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Contracts.Dtos;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Commands.SubmitDraftImages;

public sealed record SubmitDraftImagesRequest(
    Guid DraftId,
    IReadOnlyList<ImageData> Images
) : ICommand<AppConc.Response<DraftStepResponse>>;
