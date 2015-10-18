using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Device.Public.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DeviceMPublicAPI.Controllers
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

        [Route("device/registrations/id/{id}")]
        [HttpGet]
        public Registration GetById(string id)
        {
            return _deviceM.GetById(id);
        }

        [Route("device/registrations/participant/{id}")]
        [HttpGet]
        public Registration GetByParticipantId(string id)
        {
            return _deviceM.GetByParticipantId(id);
        }

        [Route("device/registrations/model/{model}")]
        [HttpGet]
        public Registrations GetByModel(string model)
        {
            return _deviceM.GetByModel(model);
        }
    }
}
