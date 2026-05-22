using Turbo.Module.Catalog.Domain.Attributes;

namespace Turbo.Module.Catalog.Domain.Enum;

public enum Model
{
    // Toyota
    [Brand(Brand.Toyota)]
    Camry,

    [Brand(Brand.Toyota)]
    Corolla,

    [Brand(Brand.Toyota)]
    RAV4,

    // BMW
    [Brand(Brand.BMW)]
    Series3,

    [Brand(Brand.BMW)]
    Series5,

    [Brand(Brand.BMW)]
    X5,

    // Mercedes
    [Brand(Brand.Mercedes)]
    CClass,

    [Brand(Brand.Mercedes)]
    EClass,

    [Brand(Brand.Mercedes)]
    GLE,

    // Hyundai
    [Brand(Brand.Hyundai)]
    Tucson,

    [Brand(Brand.Hyundai)]
    Elantra,

    // Ford
    [Brand(Brand.Ford)]
    Mustang,

    [Brand(Brand.Ford)]
    Focus,
}