using System.Collections.Generic;
using LooksFamiliar.Microservices.Ref.Models;

namespace LooksFamiliar.Microservices.Ref.Admin.Interface
{
    public interface IRefAdmin
    {
        Entities GetAll();
        Entities GetAll(string id);
        Entity Create(Entity entity);
        Entity Update(Entity entity);
        void Delete(string id);
    }
}
