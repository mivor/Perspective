using System;
using System.Collections.Generic;
using Perspective.Common.Messaging;

namespace Perspective.Common
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }
        IReadOnlyList<DomainEvent> UncommittedEvents { get; } 
        void LoadFromEvents(IEnumerable<DomainEvent> events);
        void ClearUncommittedEvents();
    }
}