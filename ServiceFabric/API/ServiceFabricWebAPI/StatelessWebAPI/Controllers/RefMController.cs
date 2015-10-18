using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Ref.Public.Service;

namespace StatelessWebAPI.Controllers
{
    public class RefMController : ApiController
    {
        private readonly RefM _refM;

        public RefMController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            //var docdburi = ConfigurationManager.AppSettings["docdburi"];
            //var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            //var redisuri = ConfigurationManager.AppSettings["redisuri"];

            var docdburi = "https://looksfamiliar.documents.azure.com:443/";
            var docdbkey = "gT/fIiFb0F+gloeYwF6HdvpNpStTk9nqZpFGl76C+k/bbPTh3TZtW4Y8k1lUXH9SKvj1nibhuKMpa0ECLAEvbQ==";
            var redisuri = "looksfamiliar.redis.cache.windows.net,ssl=true,password=Qe/Vp95SzAPINd/Mh515CQvppGKyn+u+hHCUXZJBq0Q=";

            _refM = new RefM(docdburi, docdbkey, redisuri);
        }

        [HttpGet]
        public List<Entity> GetAllByDomain(string domain)
        {
            return _refM.GetAllByDomain(domain);
        }

        [HttpGet]
        public Entity GetByCode(string code)
        {
            return _refM.GetByCode(code);
        }

        [HttpGet]
        public List<Entity> GetByCodeValue(string codevalue)
        {
            return _refM.GetByCodeValue(codevalue);
        }

        [HttpGet]
        public List<Entity> GetAllByLink(string link)
        {
            return _refM.GetAllByLink(link);
        }
    }
}
