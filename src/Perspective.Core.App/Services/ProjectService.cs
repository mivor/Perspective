using System;
using Perspective.Common;
using Perspective.Common.Messaging;
using Perspective.Core.Aggregates;
using Perspective.Core.App.Commands;

namespace Perspective.Core.App.Services
{
    public sealed class ProjectService : IHandle<AddProject>, IHandle<RenameProject>
    {
        private readonly Func<IRepository> _repoFactory;

        public ProjectService(Func<IRepository> repoFactory)
        {
            _repoFactory = repoFactory;
        }

        public void Handle(AddProject command)
        {
            var project = Project.Add(command.Name, command.ColorHex);

            var repository = _repoFactory();

            repository.SaveAsync(project).Wait();
        }

        public void Handle(RenameProject command)
        {
            ExecuteCommand(command.Id, project => project.Rename(command.NewName));
        }

        private void ExecuteCommand(Guid id, Action<Project> behavior)
        {
            var repository = _repoFactory();
            var project = repository.GetByIdAsync<Project>(id).Result;
            behavior(project);
            repository.SaveAsync(project).Wait();
        }
    }
}