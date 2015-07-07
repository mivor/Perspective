using System;
using Perspective.Common.Messaging;

namespace Perspective.Core.App.Commands
{
    public class AddProject : Command
    {
        public readonly string Name;
        public readonly string ColorHex;

        public AddProject(string name, string colorHex)
        {
            Name = name;
            ColorHex = colorHex;
        }
    }

    public class RenameProject : Command
    {
        public readonly Guid Id;
        public readonly string NewName;

        public RenameProject(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }
}