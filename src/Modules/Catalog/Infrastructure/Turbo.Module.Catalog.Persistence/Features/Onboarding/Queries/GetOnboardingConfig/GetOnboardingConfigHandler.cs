using Turbo.Module.Catalog.Domain.Enum;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Queries.GetOnboardingConfig;

public sealed class GetOnboardingConfigHandler
    : IQueryHandler<GetOnboardingConfigRequest, AppConc.Response<GetOnboardingConfigResponse>>
{
    public Task<AppConc.Response<GetOnboardingConfigResponse>> HandleAsync(
        GetOnboardingConfigRequest query,
        CancellationToken ct = default)
    {
        var fuelTypes = Enum.GetValues<FuelType>()
            .Select(f => new SelectOption((int)f, f.ToString()))
            .ToList();

        var transmissions = Enum.GetValues<TransmissionType>()
            .Select(t => new SelectOption((int)t, t.ToString()))
            .ToList();

        var steps = new List<OnboardingStep>
        {
            new(1, "images", "Şəkillər əlavə edin",
            [
                new("images", "Şəkillər", "file", Required: true,
                    Multiple: true, Accept: "image/jpeg,image/png,image/webp,image/gif")
            ]),
            new(2, "details", "Avtomobil məlumatları",
            [
                new("brandId", "Marka", "select", Required: true,
                    OptionsUrl: "/api/brands"),
                new("modelId", "Model", "select", Required: true,
                    OptionsUrl: "/api/models?brandId={brandId}"),
                new("year", "İl", "number", Required: true,
                    Min: 1886, Max: DateTime.UtcNow.Year),
                new("mileage", "Yürüş (km)", "number", Required: true, Min: 0),
                new("fuelType", "Yanacaq", "select", Required: true, Options: fuelTypes),
                new("transmissionType", "Sürətlər qutusu", "select", Required: true,
                    Options: transmissions)
            ]),
            new(3, "pricing", "Qiymət və təsvir",
            [
                new("price", "Qiymət (AZN)", "number", Required: true, Min: 1),
                new("description", "Təsvir", "textarea", Required: true, MaxLength: 2000)
            ])
        };

        return Task.FromResult(
            AppConc.Response<GetOnboardingConfigResponse>.Success(
                new GetOnboardingConfigResponse(steps)));
    }
}
