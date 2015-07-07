using System;
using Perspective.Common;
using Perspective.Common.Messaging;
using Perspective.Common.Utils;
using Perspective.Core.Events;

namespace Perspective.Core.Aggregates
{
    public sealed class Project : Aggregate
    {
        private Guid _id;

        public override Guid Id
        {
            get { return _id; }
        }

        public static Project Add(string name, string colorHex)
        {
            return new Project(name, colorHex);
        }

        private Project(string name, string colorHex)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNullOrEmpty(colorHex, "colorHex");

            ApplyChange(new ProjectAdded(Guid.NewGuid(), name, colorHex));
        }

        public void Rename(string newName)
        {
            Ensure.NotNullOrEmpty(newName, "newName");

            ApplyChange(new ProjectRenamed(Id, newName));
        }

        private void Apply(ProjectAdded e)
        {
            _id = e.Id;
        }

        private void Apply(ProjectRenamed e)
        {
            // TODO
        }

        protected override void ApplyEvent(DomainEvent evnt)
        {
            Apply((dynamic)evnt);
        }
    }
}