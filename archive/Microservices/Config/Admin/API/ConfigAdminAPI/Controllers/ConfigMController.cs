using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LooksFamiliar.Microservices.Config.Admin.Interface;
using LooksFamiliar.Microservices.Config.Admin.Service;
using LooksFamiliar.Microservices.Config.Models;
using Microsoft.Ajax.Utilities;

namespace ConfigAdminAPI.Controllers
{
    public class ConfigMController : ApiController
    {
        private readonly IConfigAdmin _configM;

        public ConfigMController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _configM = new ConfigM(docdburi, docdbkey, redisuri);
        }

        [Route("config/manifests")]
        [HttpPost]
        public Manifest Create([FromBody] Manifest manifests)
        {
            return _configM.Create(manifests);
        }

        [Route("config/manifests")]
        [HttpPut]
        public Manifest Update([FromBody] Manifest manifests)
        {
            return _configM.Update(manifests);
        }

        [Route("config/manifests")]
        [HttpGet]
        public Manifests GetAll()
        {
            return _configM.GetAll();
        }

        [Route("config/manifests/id/{id}")]
        [HttpGet]
        public Manifests GetAll(string id)
        {
            return _configM.GetAll(id);
        }

        [Route("config/manifests/id/{id}")]
        [HttpDelete]
        public void Delete(string id)
        {
            _configM.Delete(id);
        }
    }
}
