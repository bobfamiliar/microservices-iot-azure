using System.Collections.Generic;
using LooksFamiliar.Microservices.Profile.Models;

namespace LooksFamiliar.Microservices.Profile.Public.Interface
{
    public interface IProfile
    {
        UserProfile Create(UserProfile profile);
        UserProfile Update(UserProfile profile);
        UserProfile GetById(string id);
        UserProfile GetByName(string firstname, string lastname);
        List<UserProfile> GetByState(string state);
        List<UserProfile> GetAllByType(string type);
    }
}
