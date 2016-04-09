using Looksfamiliar.d2c2d.MessageModels;

namespace LooksFamiliar.Microservices.Provision.Interface
{
    public interface IProvision
    {
        DeviceManifests GetAll();
        DeviceManifest GetById(string id);
        DeviceManifest Create(DeviceManifest manifest);
        DeviceManifest Update(DeviceManifest manifest);
    }
}
