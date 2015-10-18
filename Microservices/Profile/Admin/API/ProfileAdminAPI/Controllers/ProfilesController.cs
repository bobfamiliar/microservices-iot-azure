using System.Collections.Generic;
using System.Web.Http;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Profile.Admin.Service;
using System.Configuration;

namespace ProfileAdminAPI.Controllers
{
    public class ProfilesController : ApiController
    {
        private readonly ProfileM _profileM;

        public ProfilesController()
        {
            // the configuration information comes from Web.Config when 
            // debugging and from the Azure Portal at runt time
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _profileM = new ProfileM(docdburi, docdbkey, redisuri);  
        }

        [Route("profile/users")]
        [HttpGet]
        public UserProfiles GetAll()
        {
            return _profileM.GetAll();
        }

        [Route("profile/users/id/{id}")]
        [HttpGet]
        public UserProfiles GetAll(string id)
        {
            return _profileM.GetAll(id);
        }

        [Route("profile/users/id/{id}")]
        [HttpDelete]
        public void Delete(string id)
        {
            _profileM.Delete(id);
        }
    }
}
