using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Catalog.Domain.Entity;

public class Brand : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public Brand(string name) : base(Guid.NewGuid())
    {
        Name = name;
    }

    protected Brand() : base(Guid.NewGuid()) { } // EF Core

    public void UpdateName(string name) => Name = name;
}