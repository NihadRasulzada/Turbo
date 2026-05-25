using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Catalog.Domain.Entity;

public class Model : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid BrandId { get; private set; }

    public Model(string name, Guid brandId) : base(Guid.NewGuid())
    {
        Name = name;
        BrandId = brandId;
    }

    protected Model() : base(Guid.NewGuid()) { } // EF Core

    public void Update(string name, Guid brandId)
    {
        Name = name;
        BrandId = brandId;
    }
}