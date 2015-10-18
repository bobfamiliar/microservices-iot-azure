using System.Collections.Generic;
using LooksFamiliar.Microservices.Ref.Models;

namespace LooksFamiliar.Microservices.Ref.Public.Interface
{
    public interface IRefPublic
    {
        List<Entity> GetAllByDomain(string domain);
        List<Entity> GetAllByLink(string link);
        Entity GetByCode(string code);
        List<Entity> GetByCodeValue(string codevalue);
    }
}
