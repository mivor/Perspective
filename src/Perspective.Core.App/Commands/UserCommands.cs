using Perspective.Common.Messaging;

namespace Perspective.Core.App.Commands
{
    public class SignUp : Command
    {
        public readonly string Email;
        public readonly string Password;
        public readonly string FirstName;
        public readonly string LastName;

        public SignUp(string email, string password, string firstName, string lastName)
        {
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }
    }

    public class SignIn : Command
    {

    }
}