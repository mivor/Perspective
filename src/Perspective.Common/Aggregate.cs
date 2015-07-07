using System;
using System.Collections.Generic;
using Perspective.Common.Messaging;

namespace Perspective.Common
{
    public abstract class Aggregate : IAggregate
    {
        private readonly List<DomainEvent> _changes = new List<DomainEvent>();

        public abstract Guid Id { get; }
        public int Version { get; private set; }

        public IReadOnlyList<DomainEvent> UncommittedEvents
        {
            get { return _changes; }
        }

        public void ClearUncommittedEvents()
        {
            _changes.Clear();
        }

        public void LoadFromEvents(IEnumerable<DomainEvent> events)
        {
            foreach (var e in events) ProcessEvent(e);
        }

        protected void Apply(DomainEvent evnt)
        {
            throw new ArgumentException(string.Format("No Apply method found for event: {0}", evnt.GetType()));
        }

        protected abstract void ApplyEvent(DomainEvent evnt);

        protected void ApplyChange(DomainEvent evnt)
        {
            ProcessEvent(evnt);
            _changes.Add(evnt);
        }

        private void ProcessEvent(DomainEvent evnt)
        {
            ApplyEvent(evnt);
            Version++;
        }
    }
}