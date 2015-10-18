using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StatelessWebAPI.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var message = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("RefM Reliable Service Available")
            };
            return message;
        }
    }
}
