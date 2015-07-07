using System.Collections.Generic;
using Nancy.Security;

namespace Perspective.Api.Modules
{
    public class User : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }

        public User()
        {
            Claims = new List<string>();
        }
    }
}