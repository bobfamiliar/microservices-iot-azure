using System;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Config.Public.Interface;

namespace LooksFamiliar.Microservices.Config.Public.SDK
{
    public class ConfigM : IConfig
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public ConfigM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public Manifest GetById(string id)
        {
            Manifest manifest;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_CONFIGM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/manifests/id/" + id);
                    
                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                manifest = ModelManager.JsonToModel<Manifest>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_CONFIGM_MODEL_NOT_FOUND_ID, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }

        public Manifest GetByName(string name)
        {
            Manifest manifest;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_CONFIGM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/manifests/name/" + name);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                manifest = ModelManager.JsonToModel<Manifest>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_CONFIGM_MODEL_NOT_FOUND_NAME, name) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }
    }
}

