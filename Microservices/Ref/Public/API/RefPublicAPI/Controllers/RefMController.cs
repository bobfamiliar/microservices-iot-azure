using System.Collections.Generic;
using System.Web.Http;
using LooksFamiliar.Microservices.Ref.Public.Service;
using LooksFamiliar.Microservices.Ref.Models;
using System.Configuration;

namespace RefPublicAPI.Controllers
{
    public class RefMController : ApiController
    {
        private readonly RefM _refM;

        public RefMController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _refM = new RefM(docdburi, docdbkey, redisuri);
        }

        [Route("ref/entities/domain/{domain}")]
        [HttpGet]
        public List<Entity> GetAllByDomain(string domain)
        {
            return _refM.GetAllByDomain(domain);
        }

        [Route("ref/entities/code/{code}")]
        [HttpGet]
        public Entity GetByCode(string code)
        {
            return _refM.GetByCode(code);
        }

        [Route("ref/entities/codevalue/{codevalue}")]
        [HttpGet]
        public List<Entity> GetByCodeValue(string codevalue)
        {
            return _refM.GetByCodeValue(codevalue);
        }

        [Route("ref/entities/link/{link}")]
        [HttpGet]
        public List<Entity> GetAllByLink(string link)
        {
            return _refM.GetAllByLink(link);
        }
    }
}
