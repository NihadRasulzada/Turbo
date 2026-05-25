using Turbo.Module.Media.Domain.Enums;
using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Media.Domain.Entity;

public class Media : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public MediaOwnerType OwnerType { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool IsResized { get; private set; }

    public Media(Guid ownerId, MediaOwnerType ownerType, string url, string objectKey, int order)
        : base(Guid.NewGuid())
    {
        OwnerId = ownerId;
        OwnerType = ownerType;
        Url = url;
        ObjectKey = objectKey;
        Order = order;
        IsResized = false;
    }

    public void TransferOwnership(Guid newOwnerId, MediaOwnerType newOwnerType)
    {
        OwnerId = newOwnerId;
        OwnerType = newOwnerType;
    }

    public void SetResized() => IsResized = true;

    protected Media() : base(Guid.NewGuid()) { } // EF Core
}
