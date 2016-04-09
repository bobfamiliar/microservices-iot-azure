using LooksFamiliar.Microservices.Device.Models;

namespace LooksFamiliar.Microservices.Device.Public.Interface
{
    public interface IDevicePublic
    {
        Registration GetById(string id);
        Registration GetByParticipantId(string id);
        Registrations GetByModel(string model);
    }
}
