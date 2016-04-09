using System;
using Looksfamiliar.d2c2d.MessageModels;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Provision.Interface;

namespace LooksFamiliar.Microservices.Provision.SDK
{
    public class ProvisionM : IProvision
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public ProvisionM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public DeviceManifests GetAll()
        {
            DeviceManifests manifests;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROVISIONM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/devicemanifests");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                manifests = ModelManager.JsonToModel<DeviceManifests>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_PROVISIONM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifests;
        }

        public DeviceManifest GetById(string id)
        {
            DeviceManifest manifest;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROVISIONM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/devicemanifests/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                manifest = ModelManager.JsonToModel<DeviceManifest>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_PROVISIONM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }

        public DeviceManifest Create(DeviceManifest manifest)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROVISIONM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/devicemanifests");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var payload = ModelManager.ModelToJson<DeviceManifest>(manifest);

                var json = Rest.Post(uriBuilder.Uri, payload);

                manifest = ModelManager.JsonToModel<DeviceManifest>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_PROVISIONM_MODEL_NOT_CREATED, manifest.model) + ", " +
                                err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }

        public DeviceManifest Update(DeviceManifest manifest)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROVISIONM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/devicemanifests");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var payload = ModelManager.ModelToJson<DeviceManifest>(manifest);

                var json = Rest.Put(uriBuilder.Uri, payload);

                manifest = ModelManager.JsonToModel<DeviceManifest>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_PROVISIONM_MODEL_NOT_UPDATED, manifest.model) + ", " +
                                err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }
    }
}
