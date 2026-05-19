namespace Turbo.Shared.Domain.Models;

public abstract class BaseEntity(Guid id) : IBaseEntity
{
    public Guid Id { get; private set; } = id;

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}