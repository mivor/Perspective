using System;
using Perspective.Common;
using Perspective.Common.Messaging;
using Perspective.Core.Aggregates;
using Perspective.Core.App.Commands;

namespace Perspective.Core.App.Services
{
    public class UserService : IHandle<SignUp>
    {
        private readonly Func<IRepository> _repoFactory;

        public UserService(Func<IRepository> repoFactory)
        {
            _repoFactory = repoFactory;
        }

        public void Handle(SignUp cmd)
        {
            var user = User.SignUp(cmd.Email, cmd.Password, cmd.FirstName, cmd.LastName);

            var repository = _repoFactory();
            repository.SaveAsync(user).Wait();
        }
    }
}