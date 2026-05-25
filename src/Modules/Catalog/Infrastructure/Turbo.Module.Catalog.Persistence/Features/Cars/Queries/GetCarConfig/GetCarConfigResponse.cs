namespace Turbo.Module.Catalog.Persistence.Features.Cars.Queries.GetCarConfig;

public sealed record GetCarConfigResponse(IReadOnlyList<CarStep> Steps);

public sealed record CarStep(
    int StepNumber,
    string Key,
    string Title,
    IReadOnlyList<CarField> Fields
);

public sealed record CarField(
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