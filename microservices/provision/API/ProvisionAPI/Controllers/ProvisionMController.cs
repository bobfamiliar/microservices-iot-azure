using System.Web.Http;
using System.Configuration;
using Looksfamiliar.d2c2d.MessageModels;
using LooksFamiliar.Microservices.Provision.Service;

namespace ProvisionAPI.Controllers
{
    public class ProvisionController : ApiController
    {
        private readonly ProvisionM _provisionM;

        public ProvisionController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var iothubconnstr = ConfigurationManager.AppSettings["iothubconnstr"];

            _provisionM = new ProvisionM(docdburi, docdbkey, iothubconnstr);
        }

        [Route("provision/devicemanifests")]
        [RequireHttps]
        [HttpGet]
        public DeviceManifests GetAll()
        {
            return _provisionM.GetAll();
        }

        [Route("provision/devicemanifests")]
        [RequireHttps]
        [HttpPost]
        public DeviceManifest Create([FromBody] DeviceManifest manifest)
        {
            return _provisionM.Create(manifest);
        }

        [Route("provision/devicemanifests")]
        [RequireHttps]
        [HttpPut]
        public DeviceManifest Update([FromBody] DeviceManifest manifest)
        {
            return _provisionM.Update(manifest);
        }
    }
}
