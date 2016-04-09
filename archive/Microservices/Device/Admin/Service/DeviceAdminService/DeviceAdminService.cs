using System;
using LooksFamiliar.Microservices.Device.Admin.Interface;
using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Common.Store;
using System.Collections.Generic;

namespace LooksFamiliar.Microservices.Device.Admin.Service
{
    public class DeviceM : IDeviceAdmin
    {
        private readonly IPersist _persist;

        public DeviceM(string docdburi, string docdbkey, string redisuri)
        {
            this._persist = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            this._persist.Connect("DeviceM", "Registry");
        }

        public Registration Create(Registration device)
        {
            try
            {
                if (device.isValid())
                    this._persist.Insert(device);
                else
                {
                    throw new Exception(Errors.ERR_DEVICEM_MODEL_NOT_VALID);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BAGREQUEST, err);
            }

            return device;
        }

        public Registration Update(Registration device)
        {
            try
            {
                if (device.isValid())
                    this._persist.Update(device);
                else
                {
                    throw new Exception(Errors.ERR_DEVICEM_MODEL_NOT_VALID);
                }
            }
            catch (Exception err)
            {
                throw new Exception(string.Format(Errors.ERR_DEVICEM_MODEL_NOT_UPDATED, device.model), err);
            }

            return device;
        }

        public void Delete(string id)
        {
            try
            {
                this._persist.Delete(id);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_MODEL_NOT_DELETED, err);
            }
        }

        public Registrations GetAll()
        {
            Registrations devices = new Registrations();

            try
            {
                List<Registration> deviceList = this._persist.SelectAll<Registration>();
                foreach (Registration d in deviceList)
                {
                    devices.list.Add(d);
                }
                this._persist.InsertCache(devices);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BAGREQUEST, err);
            }

            return devices;
        }

        public Registrations GetAll(string id)
        {
            Registrations devices;

            try
            {
                // this will return the list from cache or if null, get from the db
                devices = this._persist.SelectById<Registrations>(id);
                if (devices == null)
                    return GetAll();
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BAGREQUEST, err);
            }

            return devices;
        }
    }
}
