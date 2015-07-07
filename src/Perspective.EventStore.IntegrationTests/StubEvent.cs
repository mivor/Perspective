using System;
using Perspective.Common.Messaging;

namespace Perspective.EventStore.IntegrationTests
{
    public class StubEvent : DomainEvent
    {
        public readonly string Prop1;
        public readonly string Prop2;

        public StubEvent(string prop1, string prop2)
        {
            Prop1 = prop1;
            Prop2 = prop2;
        }
    }

    public class TestAggregateCreated : DomainEvent
    {
        public readonly Guid AggregateId;

        public TestAggregateCreated(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}