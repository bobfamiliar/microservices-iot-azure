using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProvisionAPI
{
    public class ApiKeyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (ValidateKey(request)) return base.SendAsync(request, cancellationToken);
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        private bool ValidateKey(HttpRequestMessage message)
        {
            var headers = message.Headers;
            if (!headers.Contains("apiss")) return false;
            var theirApiKey = headers.GetValues("apiss").First();
            var myApiKey = ConfigurationManager.AppSettings["apiss"];
            return (theirApiKey == myApiKey);
        }
    }
}
