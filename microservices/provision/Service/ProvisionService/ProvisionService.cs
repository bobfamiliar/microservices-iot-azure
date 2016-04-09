using System;
using System.Threading;
using LooksFamiliar.Microservices.Common.Store;
using System.Threading.Tasks;
using Looksfamiliar.d2c2d.MessageModels;
using LooksFamiliar.Microservices.Provision.Interface;
using Microsoft.Azure.Devices;

namespace LooksFamiliar.Microservices.Provision.Service
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
          TaskFactory(CancellationToken.None,
                      TaskCreationOptions.None,
                      TaskContinuationOptions.None,
                      TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._myTaskFactory
              .StartNew<Task<TResult>>(func)
              .Unwrap<TResult>()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            AsyncHelper._myTaskFactory
              .StartNew<Task>(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }
    }

    public class ProvisionM : IProvision
    {
        private readonly IDbase _dbase;
        private readonly RegistryManager _registryManager;
        private Device _device;
    
        public ProvisionM(string docdburi, string docdbkey, string iothubconnstr)
        {
            this._registryManager = RegistryManager.CreateFromConnectionString(iothubconnstr);
            this._dbase = new Dbase(docdburi, docdbkey);
            this._dbase.Connect("Device", "Registry");
        }

        public DeviceManifest Create(DeviceManifest manifest)
        {
            try
            {
                if (manifest.isValid())
                {
                    // Provision the device in IoT Hub
                    var device = AsyncHelper.RunSync<Device>(() => _registryManager.AddDeviceAsync(new Device(manifest.serialnumber)));

                    // Update the manifest and store in DocDb
                    manifest.key = device.Authentication.SymmetricKey.PrimaryKey;
                    _dbase.Insert(manifest);
                }
                else
                {
                    throw new Exception(Errors.ERR_DEVICEM_MODEL_NOT_VALID);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BAGREQUEST, err);
            }

            return manifest;
        }

        public DeviceManifest Update(DeviceManifest manifest)
        {
            try
            {
                if (manifest.isValid())
                    this._dbase.Update(manifest);
                else
                {
                    throw new Exception(Errors.ERR_DEVICEM_MODEL_NOT_VALID);
                }
            }
            catch (Exception err)
            {
                throw new Exception(string.Format(Errors.ERR_DEVICEM_MODEL_NOT_UPDATED, manifest.model), err);
            }

            return manifest;
        }

        public DeviceManifests GetAll()
        {
            var devices = new DeviceManifests();

            try
            {
                var deviceList = this._dbase.SelectAll<DeviceManifest>();
                foreach (var d in deviceList)
                {
                    devices.list.Add(d);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BAGREQUEST, err);
            }

            return devices;
        }

        public DeviceManifest GetById(string id)
        {
            DeviceManifest manifest = null;

            try
            {
                var query = $"select * from DeviceManifest d where d.serialnumber='{id}'";
                var manifestList = _dbase.SelectByQuery<DeviceManifest>(query);
                if (manifestList[0] != null)
                {
                    manifest = manifestList[0];
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BAGREQUEST, err);
            }

            return manifest;
        }
    }
}
