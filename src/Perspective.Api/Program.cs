using System;
using Nancy.Hosting.Self;

namespace Perspective.Api
{
    public class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:4000");
            var config = new HostConfiguration
            {
                RewriteLocalhost = false,
            };

            using (var host = new NancyHost(uri, new Bootstrapper(), config))
            {
                host.Start();

                Console.WriteLine("Your application is running on " + uri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}