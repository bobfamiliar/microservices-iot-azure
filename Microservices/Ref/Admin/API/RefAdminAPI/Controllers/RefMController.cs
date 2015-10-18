using System;
using System.Collections.Generic;
using System.Web.Http;
using LooksFamiliar.Microservices.Ref.Admin.Service;
using LooksFamiliar.Microservices.Ref.Models;
using System.Configuration;

namespace RefAdminAPI.Controllers
{
    public class RefMController : ApiController
    {
        private readonly RefM _refM;

        public RefMController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at run time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _refM = new RefM(docdburi, docdbkey, redisuri);
        }

        [Route("ref/entities")]
        [HttpGet]
        public Entities GetAll()
        {
            return _refM.GetAll();
        }

        [Route("ref/entities/id/{id}")]
        [HttpGet]
        public Entities GetAll(string id)
        {
            return _refM.GetAll(id);
        }

        [Route("ref/entities")]
        [HttpPost]
        public Entity Create([FromBody] Entity entity)
        {
            return _refM.Create(entity);
        }

        [Route("ref/entities")]
        [HttpPut]
        public Entity Update([FromBody] Entity entity)
        {
            return _refM.Update(entity);
        }

        [Route("ref/entities/id/{id}")]
        [HttpDelete]
        public void Delete(string id)
        {
            _refM.Delete(id);
        }
    }
}
