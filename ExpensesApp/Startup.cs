using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using ExpensesApp.Providers;

[assembly: OwinStartup(typeof(ExpensesApp.Startup))]

namespace ExpensesApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // WebAPI Config
            var config = new HttpConfiguration();

            // Attribute Routing
            config.MapHttpAttributeRoutes();
                
            // Changing XML format to JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.Indent = true;

            EnableTokenAccess(app);

            // Enabling CORS
            app.UseCors(CorsOptions.AllowAll);

            // Enabling WebAPI Config
            app.UseWebApi(config);
        }

        private void EnableTokenAccess(IAppBuilder app)
        {
            var tokenOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new AccessTokenProvider()
            };
            app.UseOAuthAuthorizationServer(tokenOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
