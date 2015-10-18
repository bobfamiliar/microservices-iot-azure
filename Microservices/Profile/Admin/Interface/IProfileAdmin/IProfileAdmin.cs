using LooksFamiliar.Microservices.Profile.Models;

namespace LooksFamiliar.Microservices.Profile.Admin.Interface
{
    public interface IProfileAdmin
    {
        UserProfiles GetAll();
        UserProfiles GetAll(string id);
        void Delete(string id);
    }
}
