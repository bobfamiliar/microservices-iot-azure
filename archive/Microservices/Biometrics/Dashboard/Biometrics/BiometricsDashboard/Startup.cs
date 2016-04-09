using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BiometricsDashboard.Startup))]
namespace BiometricsDashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            //ConfigureAuth(app);
        }
    }
}
