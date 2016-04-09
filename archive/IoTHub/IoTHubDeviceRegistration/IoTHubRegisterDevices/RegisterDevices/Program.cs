using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using LooksFamiliar.Microservices.Config.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using LooksFamiliar.Microservices.Config.Public.SDK;
using LooksFamiliar.Microservices.Device.Admin.SDK;
using LooksFamiliar.Microservices.Device.Models;

namespace LooksFamiliar.DeviceManagement.BioMax.IoTHub
{
    class Program
    {
        static ConfigM _configM;
        static DeviceM _registryM;
        static Registrations _devices;
        static RegistryManager _registryManager;
        private static int _count;

        static void Main(string[] args)
        {
            // initialize the configM microservice client sdk
            _configM = new ConfigM {ApiUrl = ConfigurationManager.AppSettings["ConfigM"]};

            // lookup the manifest for the device microservice
            var deviceManifest = _configM.GetByName("DeviceM");

            // initialize the device registery microservice client SDK
            _registryM = new DeviceM {ApiUrl = deviceManifest.lineitems[LineitemsKey.AdminAPI]};

            // get a list of all devices from the registry
            _devices = _registryM.GetAll();

            // initialize the IoT Hub registration manager
            _registryManager = RegistryManager.CreateFromConnectionString(ConfigurationManager.AppSettings["IoTHubConnStr"]);

            // register each device with IoT Hub
            foreach (var device in _devices.list)
            {
                AddDeviceAsync(device).Wait();
            }
        }

        private async static Task AddDeviceAsync(Registration deviceRegistration)
        {
            //if (deviceRegistration.key != string.Empty) return;

            // create a unique id for this device
            var deviceId = deviceRegistration.model + "-" + deviceRegistration.id;

            // this class represents the device registered with IoT Hub
            Device device;

            // register or lookup the device
            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));
                device.Authentication.SymmetricKey;
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceId);
            }

            try
            {
                // update the application registry
                deviceRegistration.key = device.Authentication.SymmetricKey.PrimaryKey;
                _registryM.Update(deviceRegistration);
                Console.WriteLine("{0} Generated device {1} key: {2}", _count++, deviceRegistration.model, device.Authentication.SymmetricKey.PrimaryKey);
                Thread.Sleep(3000);
            }
            catch (Exception err)
            {
                Console.WriteLine("{0} {1}",  _count++, err.Message);
            }
        }
    }
}
