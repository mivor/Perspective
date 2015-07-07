using System;
using Perspective.Common.Messaging;

namespace Perspective.Core.Events
{
    public class ProjectAdded : DomainEvent
    {
        public readonly Guid Id;
        public readonly string Name;
        public readonly string ColorHex;

        public ProjectAdded(Guid id, string name, string colorHex)
        {
            Id = id;
            Name = name;
            ColorHex = colorHex;
        }
    }

    public class ProjectRenamed : DomainEvent
    {
        public readonly Guid Id;
        public readonly string NewName;

        public ProjectRenamed(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }
}