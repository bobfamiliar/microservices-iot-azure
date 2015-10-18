using Microsoft.ServiceFabric.Services;

namespace StatelessWebAPI
{
    public class StatelessWebAPI : StatelessService
    {
        public const string ServiceTypeName = "StatelessWebAPIType";

        protected override ICommunicationListener CreateCommunicationListener()
        {
            return new OwinCommunicationListener("refm", new Startup());
        }
    }
}
