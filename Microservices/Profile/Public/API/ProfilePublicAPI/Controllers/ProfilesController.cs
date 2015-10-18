using System.Collections.Generic;
using System.Web.Http;
using LooksFamiliar.Microservices.Profile.Public.Service;
using LooksFamiliar.Microservices.Profile.Models;
using System.Configuration;

namespace ProfileAPI.Controllers
{
    public class ProfilesController : ApiController
    {
        private readonly ProfileM _profileM;

        public ProfilesController()
        {
            var docdburi = ConfigurationManager.AppSettings["docdburi"];
            var docdbkey = ConfigurationManager.AppSettings["docdbkey"];
            var redisuri = ConfigurationManager.AppSettings["redisuri"];

            _profileM = new ProfileM(docdburi, docdbkey, redisuri);
        }

        [Route("profile/users")]
        [HttpPost]
        public UserProfile Create([FromBody] UserProfile profile)
        {
            return _profileM.Create(profile);
        }

        [Route("profile/users")]
        [HttpPut]
        public UserProfile Update([FromBody] UserProfile profile)
        {
            return _profileM.Update(profile);
        }
        
        [Route("profile/users/id/{id}")]
        [HttpGet]
        public UserProfile GetById(string id)
        {
            return _profileM.GetById(id);
        }

        [Route("profile/users/firstname/{firstname}/lastname/{lastname}")]
        [HttpGet]
        public UserProfile GetByName(string firstname, string lastname)
        {
            return _profileM.GetByName(firstname, lastname);
        }

        [Route("profile/users/type/{type}")]
        [HttpGet]
        public List<UserProfile> GetByType(string type)
        {
            return _profileM.GetAllByType(type);
        }

        [Route("profile/users/state/{state}")]
        [HttpGet]
        public List<UserProfile> GetByState(string state)
        {
            return _profileM.GetByState(state);
        }
    }
}
