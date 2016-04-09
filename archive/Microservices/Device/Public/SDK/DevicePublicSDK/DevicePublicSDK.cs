using System;
using LooksFamiliar.Microservices.Device.Public.Interface;
using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Common.Wire;

namespace LooksFamiliar.Microservices.Device.Public.SDK
{
    public class DeviceM : IDevicePublic
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public DeviceM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public Registration GetById(string id)
        {
            Registration device;

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

                device = ModelManager.JsonToModel<Registration>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_DEVICEM_MODEL_NOT_FOUND_ID, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return device;
        }

        public Registrations GetByModel(string model)
        {
            Registrations devices;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations/model/" + model);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                devices = ModelManager.JsonToModel<Registrations>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_DEVICEM_MODEL_NOT_FOUND_NAME, model) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return devices;
        }

        public Registration GetByParticipantId(string id)
        {
            Registration device;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_DEVICEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/registrations/participant/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                device = ModelManager.JsonToModel<Registration>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_DEVICEM_MODEL_NOT_FOUND_PARTICIPANT, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return device;
        }
    }
}
