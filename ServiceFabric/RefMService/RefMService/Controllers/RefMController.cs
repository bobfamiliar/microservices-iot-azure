using System.Collections.Generic;
using System.Web.Http;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Ref.Public.Service;

namespace RefMService.Controllers
{
    public class RefMController : ApiController
    {
        private readonly RefM _refM;

        public RefMController()
        {
            var docdburi = "[docdburi]";
            var docdbkey = "[docdbkey]";
            var redisuri = "[redis-account].redis.cache.windows.net,ssl=true,password=[redis-key]";

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
