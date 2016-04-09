using LooksFamiliar.Microservices.Config.Models;

namespace LooksFamiliar.Microservices.Config.Public.Interface
{
    public interface IConfig
    {
        Manifest GetById(string id);
        Manifest GetByName(string name);
    }
}
