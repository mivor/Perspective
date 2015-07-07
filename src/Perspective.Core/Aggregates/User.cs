using System;
using System.Security.Cryptography;
using System.Text;
using Perspective.Common;
using Perspective.Common.Messaging;
using Perspective.Common.Utils;
using Perspective.Core.Events;

namespace Perspective.Core.Aggregates
{
    public class User : Aggregate
    {
        private Guid _id;
        private string _email;
        private byte[] _passwordHash;

        public override Guid Id
        {
            get { return _id; }
        }

        public static User SignUp(string email, string password, string firstName, string lastName)
        {
            return new User(email, password, firstName, lastName);
        }

        private User(string email, string password, string firstName, string lastName)
        {
            Ensure.NotNullOrEmpty(email, "email");
            Ensure.NotNullOrEmpty(password, "password");
            Ensure.NotNullOrEmpty(firstName, "firstName");
            Ensure.NotNullOrEmpty(lastName, "lastName");

            var passwordHash = EncryptPassword(password);
            ApplyChange(new UserSignedUp(Guid.NewGuid(), email, firstName, lastName, passwordHash));
        }

        private static byte[] EncryptPassword(string password)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var data = Encoding.Unicode.GetBytes(password);
            return sha1.ComputeHash(data);
        }

        private void Apply(UserSignedUp e)
        {
            _id = e.Id;
            _email = e.Email;
            _passwordHash = e.PasswordHash;
        }

        protected override void ApplyEvent(DomainEvent evnt)
        {
            Apply((dynamic)evnt);
        }
    }
}