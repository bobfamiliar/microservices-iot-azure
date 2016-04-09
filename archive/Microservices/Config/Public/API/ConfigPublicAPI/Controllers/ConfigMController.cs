using System.Configuration;
using System.Web.Http;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Public.Interface;
using LooksFamiliar.Microservices.Config.Public.Service;

namespace ConfigPublicAPI.Controllers
{
    public class ConfigMController : ApiController
    {
        private readonly IConfig _configM;

        public ConfigMController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _configM = new ConfigM(docdburi, docdbkey, redisuri);
        }

        [Route("config/manifests/id/{id}")]
        [HttpGet]
        public Manifest GetById(string id)
        {
            return _configM.GetById(id); ;
        }

        [Route("config/manifests/name/{name}")]
        [HttpGet]
        public Manifest GetByName(string name)
        {
            return _configM.GetByName(name);
        }
    }
}
