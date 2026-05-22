using Turbo.Module.Catalog.Domain.Enum;

namespace Turbo.Module.Catalog.Domain.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class BrandAttribute(Brand brand) : Attribute
{
    public Brand Brand { get; } = brand;
}