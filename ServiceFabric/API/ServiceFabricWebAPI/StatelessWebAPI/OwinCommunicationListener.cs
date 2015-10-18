using System;
using System.Diagnostics;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services;

namespace StatelessWebAPI
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly IOwinAppBuilder _startup;
        private readonly string _appRoot;
        private IDisposable _serverHandle;
        private string _listeningAddress;
        private string _publishAddress;

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup)
        {
            this._startup = startup;
            this._appRoot = appRoot;
        }

        public void Initialize(ServiceInitializationParameters serviceInitializationParameters)
        {
            var serviceEndpoint = serviceInitializationParameters.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var port = serviceEndpoint.Port;

            this._listeningAddress = string.Format(
                CultureInfo.InvariantCulture,
                "http://+:{0}/{1}",
                port,
                string.IsNullOrWhiteSpace(this._appRoot)
                    ? string.Empty
                    : this._appRoot.TrimEnd('/') + '/');

            this._publishAddress = this._listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.Message("Opening on {0}", this._publishAddress);

            try
            {
                ServiceEventSource.Current.Message("Starting web server on {0}", this._listeningAddress);

                this._serverHandle = WebApp.Start(this._listeningAddress, appBuilder => this._startup.Configuration(appBuilder));

                return Task.FromResult(this._publishAddress);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

                this.StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            this.StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            this.StopWebServer();
        }

        private void StopWebServer()
        {
            if (this._serverHandle != null)
            {
                try
                {
                    this._serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
