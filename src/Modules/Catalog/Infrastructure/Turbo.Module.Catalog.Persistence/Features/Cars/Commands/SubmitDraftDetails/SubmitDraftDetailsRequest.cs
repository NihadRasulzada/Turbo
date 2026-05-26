using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Cars.Commands.SubmitDraftDetails;

public sealed record SubmitDraftDetailsRequest(
    Guid DraftId,
    Guid BrandId,
    Guid ModelId,
    short Year,
    FuelType FuelType,
    TransmissionType TransmissionType,
    int Mileage,
    Guid RequesterId
) : ICommand<AppConc.Response<SubmitDraftDetailsResponse>>;
