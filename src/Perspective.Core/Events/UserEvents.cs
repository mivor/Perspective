using System;
using Perspective.Common.Messaging;

namespace Perspective.Core.Events
{
    public class UserSignedUp : DomainEvent
    {
        public readonly Guid Id;
        public readonly string Email;
        public readonly string FirstName;
        public readonly string LastName;
        public readonly byte[] PasswordHash;

        public UserSignedUp(
            Guid id, 
            string email, 
            string firstName, 
            string lastName, 
            byte[] passwordHash)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }
    }
}