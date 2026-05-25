using System;
using System.Collections.Generic;
using System.Text;

namespace Turbo.Module.Identity.Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
