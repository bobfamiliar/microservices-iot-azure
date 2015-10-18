using System.Web.Http;
using Owin;
using StatelessWebAPI.App_Start;
using StatelessWebAPI.App_Start.WebApi;

namespace StatelessWebAPI
{
    public class Startup : IOwinAppBuilder
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            FormatterConfig.ConfigureFormatters(config.Formatters);
            RouteConfig.RegisterRoutes(config.Routes);

            appBuilder.UseWebApi(config);
        }
    }
}
