using System;
using Perspective.Common;
using Perspective.Common.Messaging;

namespace Perspective.EventStore.IntegrationTests
{
    public class TestAggregate : Aggregate
    {
        private Guid _id;

        public TestAggregate(Guid aggregateId)
            : this()
        {
            ApplyChange(new TestAggregateCreated(aggregateId));
        }

        private TestAggregate()
        {
        }

        public int AppliedEventCount { get; private set; }

        public void ProduceEvents(int count)
        {
            for (int i = 0; i < count; i++)
                ApplyChange(new StubEvent("Prop1-" + i, "Prop2-" + i));
        }

        private void Apply(TestAggregateCreated evnt)
        {
            _id = evnt.AggregateId;
        }

        private void Apply(StubEvent evnt)
        {
            AppliedEventCount++;
        }

        public override Guid Id
        {
            get { return _id; }
        }

        protected override void ApplyEvent(DomainEvent evnt)
        {
            Apply((dynamic)evnt);
        }
    }
}