using System.Web.Http;
using LooksFamiliar.Microservices.Device.Admin.Service;
using LooksFamiliar.Microservices.Device.Models;
using System.Configuration;

namespace DeviceMAdminAPI.Controllers
{
    public class DeviceMController : ApiController
    {
        private readonly DeviceM _deviceM;

        public DeviceMController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _deviceM = new DeviceM(docdburi, docdbkey, redisuri);
        }

        [Route("device/registrations")]
        [HttpGet]
        public Registrations GetAll()
        {
            return _deviceM.GetAll();
        }

        [Route("device/registrations/id/{id}")]
        [HttpGet]
        public Registrations GetAll(string id)
        {
            return _deviceM.GetAll(id);
        }

        [Route("device/registrations")]
        [HttpPost]
        public Registration Create([FromBody] Registration device)
        {
            return _deviceM.Create(device);
        }

        [Route("device/registrations")]
        [HttpPut]
        public Registration Update([FromBody] Registration device)
        {
            return _deviceM.Update(device);
        }

        [Route("device/registrations/id/{id}")]
        [HttpDelete]
        public void Delete(string id)
        {
            _deviceM.Delete(id);
        }
    }
}
