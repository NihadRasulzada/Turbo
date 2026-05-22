using System.Reflection;
using Turbo.Module.Catalog.Domain.Attributes;
using Turbo.Module.Catalog.Domain.Enum;

namespace Turbo.Module.Catalog.Domain.Extensions;

public static class ModelExtensions
{
    private static readonly Dictionary<Model, Brand> _cache = System
        .Enum.GetValues<Model>()
        .Select(m => new
        {
            Model = m,
            Attr = m.GetType().GetField(m.ToString())!.GetCustomAttribute<BrandAttribute>(),
        })
        .Where(x => x.Attr is not null)
        .ToDictionary(x => x.Model, x => x.Attr!.Brand);

    public static Brand GetBrand(this Model model) =>
        _cache.TryGetValue(model, out var brand)
            ? brand
            : throw new InvalidOperationException($"{model} has no Brand attribute."); //TODO: exception lazim deyil

    public static IEnumerable<Model> GetModels(this Brand brand) =>
        _cache.Where(kv => kv.Value == brand).Select(kv => kv.Key);

    public static bool BelongsTo(this Model model, Brand brand) => model.GetBrand() == brand;
}