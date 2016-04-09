using System;
using LooksFamiliar.Microservices.Device.Admin.Interface;
using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Common.Wire;

namespace LooksFamiliar.Microservices.Device.Admin.SDK
{
    public class DeviceM : IDeviceAdmin
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public DeviceM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public Registrations GetAll()
        {
            Registrations devices;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                devices = ModelManager.JsonToModel<Registrations>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_DEVICEM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return devices;
        }

        public Registrations GetAll(string id)
        {
            Registrations devices;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                devices = ModelManager.JsonToModel<Registrations>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_DEVICEM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return devices;
        }

        public Registration Create(Registration device)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var payload = ModelManager.ModelToJson<Registration>(device);

                var json = Rest.Post(uriBuilder.Uri, payload);

                device = ModelManager.JsonToModel<Registration>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_DEVICEM_MODEL_NOT_CREATED, device.model) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return device;
        }

        public Registration Update(Registration device)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var payload = ModelManager.ModelToJson<Registration>(device);

                var json = Rest.Put(uriBuilder.Uri, payload);

                device = ModelManager.JsonToModel<Registration>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_DEVICEM_MODEL_NOT_UPDATED, device.model) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return device;
        }

        public void Delete(string id)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                Rest.Delete(uriBuilder.Uri);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_DEVICEM_MODEL_NOT_DELETED, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }
        }
    }
}
