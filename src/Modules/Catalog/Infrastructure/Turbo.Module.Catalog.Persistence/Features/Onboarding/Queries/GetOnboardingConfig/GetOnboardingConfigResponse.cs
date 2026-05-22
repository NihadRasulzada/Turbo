namespace Turbo.Module.Catalog.Persistence.Features.Onboarding.Queries.GetOnboardingConfig;

public sealed record GetOnboardingConfigResponse(IReadOnlyList<OnboardingStep> Steps);

public sealed record OnboardingStep(
    int StepNumber,
    string Key,
    string Title,
    IReadOnlyList<OnboardingField> Fields
);

public sealed record OnboardingField(
    string Name,
    string Label,
    string Type,
    bool Required,
    int? Min = null,
    int? Max = null,
    int? MaxLength = null,
    bool? Multiple = null,
    string? Accept = null,
    IReadOnlyList<SelectOption>? Options = null,
    string? OptionsUrl = null
);

public sealed record SelectOption(int Value, string Label);