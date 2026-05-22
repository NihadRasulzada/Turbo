using Turbo.Shared.Domain.Models;

namespace Turbo.Module.Media.Domain.Entity;

public class CarDraftImage : BaseEntity
{
    public Guid DraftId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public int Order { get; private set; }

    public CarDraftImage(Guid draftId, string url, string objectKey, int order)
        : base(Guid.NewGuid())
    {
        DraftId = draftId;
        Url = url;
        ObjectKey = objectKey;
        Order = order;
    }

    protected CarDraftImage() : base(Guid.NewGuid()) { } // EF Core
}