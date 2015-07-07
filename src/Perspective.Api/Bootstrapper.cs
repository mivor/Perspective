using Nancy;
using Nancy.Authentication.Token;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Perspective.Api
{
    public sealed class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ITokenizer>(new Tokenizer());
            // Example options for specifying additional values for token generation

            //container.Register<ITokenizer>(new Tokenizer(cfg =>
            //                                             cfg.AdditionalItems(
            //                                                 ctx =>
            //                                                 ctx.Request.Headers["X-Custom-Header"].FirstOrDefault(),
            //                                                 ctx => ctx.Request.Query.extraValue)));
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => 
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Authorization, Accept, Origin, Content-type"));
            var authConfig = new TokenAuthenticationConfiguration(container.Resolve<ITokenizer>());
            TokenAuthentication.Enable(pipelines, authConfig);
        }
    }
}