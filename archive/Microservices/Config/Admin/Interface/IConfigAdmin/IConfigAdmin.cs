using LooksFamiliar.Microservices.Config.Models;

namespace LooksFamiliar.Microservices.Config.Admin.Interface
{
    public interface IConfigAdmin
    {
        Manifests GetAll();
        Manifests GetAll(string id);
        Manifest Create(Manifest model);
        Manifest Update(Manifest model);
        void Delete(string id);
    }
}
