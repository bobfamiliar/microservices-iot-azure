using LooksFamiliar.Microservices.Device.Models;

namespace LooksFamiliar.Microservices.Device.Admin.Interface
{
    public interface IDeviceAdmin
    {
        Registrations GetAll();
        Registrations GetAll(string id);
        Registration Create(Registration device);
        Registration Update(Registration device);
        void Delete(string id);
    }
}
