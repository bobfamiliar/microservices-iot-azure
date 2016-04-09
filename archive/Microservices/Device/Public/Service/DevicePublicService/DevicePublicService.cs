using LooksFamiliar.Microservices.Device.Public.Interface;
using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Common.Store;
using System;

namespace LooksFamiliar.Microservices.Device.Public.Service
{
    public class DeviceM : IDevicePublic
    {
        private readonly IPersist _persist;

        public DeviceM(string docdburi, string docdbkey, string redisuri)
        {
            this._persist = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            this._persist.Connect("DeviceM", "Registry");
        }

        public Registration GetById(string id)
        {
            Registration device = null;

            try
            {
                device = this._persist.SelectById<Registration>(id);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BADREQUEST, err);
            }

            return device;
        }

        public Registrations GetByModel(string model)
        {
            Registrations devices = new Registrations();

            try
            {
                var query = "SELECT * FROM DeviceModel d WHERE d.model='" + model + "'";
                var deviceList = this._persist.SelectByQuery<Registration>(query);

                if (deviceList != null)
                {
                    foreach(var d in deviceList)
                    {
                        devices.list.Add(d);
                    }
                }
                else
                {
                    throw new Exception(Errors.ERR_DEVICEM_PROFILE_NOT_FOUND);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BADREQUEST, err);
            }

            return devices;
        }

        public Registration GetByParticipantId(string particpantid)
        {
            Registration device = null;

            try
            {
                var query = "SELECT * FROM DeviceModel d WHERE d.userprofileid='" + particpantid + "'";
                var devices = this._persist.SelectByQuery<Registration>(query);

                if (devices != null)
                {
                    device = devices[0];
                 }
                else
                {
                    throw new Exception(Errors.ERR_DEVICEM_PROFILE_NOT_FOUND);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DEVICEM_BADREQUEST, err);
            }

            return device;
        }
    }
}
