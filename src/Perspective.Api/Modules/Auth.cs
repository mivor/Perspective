using System;
using System.Net;
using EventStore.ClientAPI;
using Nancy;
using Nancy.Authentication.Token;
using Nancy.ModelBinding;
using Nancy.Security;
using Perspective.Core.App.Commands;
using Perspective.Core.App.Services;
using Perspective.EventStore;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace Perspective.Api.Modules
{
    public class SignUpDto
    {
        public string Email;
        public string Password;
        public string FirstName;
        public string LastName;
    }

    public class SignInCommand
    {
        public string Email;
        public string Password;
    }

    public class Auth : NancyModule
    {
        private static readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, 1113);

        public Auth(ITokenizer tokenizer)
            : base("/auth")
        {
            Get["/"] = _ =>
            {
                this.RequiresAuthentication();
                return HttpStatusCode.OK;
            };

            Post["/signup"] = parm =>
            {
                var dto = this.Bind<SignUpDto>();
                var command = new SignUp(dto.Email, dto.Password, dto.FirstName, dto.LastName);

                using (var conn = EventStoreConnection.Create(EndPoint))
                {
                    conn.ConnectAsync().Wait();
               
                    var userService = new UserService(() => new EventStoreRepository(conn));

                    try
                    {
                        userService.Handle(command);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                var userIdentity = new User { UserName = command.Email };
                return new
                {
                    Token = tokenizer.Tokenize(userIdentity, Context),
                };
            };

            Post["/signin"] = x =>
            {
                var cmd = this.Bind<SignInCommand>();

                var userIdentity = new User();
//                if (userIdentity == null) return HttpStatusCode.Unauthorized;

                return new
                {
                    Token = tokenizer.Tokenize(userIdentity, Context),
                };
            };
        }
    }
}