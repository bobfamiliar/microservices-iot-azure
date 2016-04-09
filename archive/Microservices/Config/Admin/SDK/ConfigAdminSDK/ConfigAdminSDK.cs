using System;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Admin.Interface;

namespace LooksFamiliar.Microservices.Config.Admin.SDK
{
    public class ConfigM : IConfigAdmin
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public ConfigM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public Manifests GetAll()
        {
            Manifests manifests;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_CONFIGM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/manifests");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                manifests = ModelManager.JsonToModel<Manifests>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_CONFIGM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifests;
        }

        public Manifests GetAll(string id)
        {
            Manifests manifests;

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

                manifests = ModelManager.JsonToModel<Manifests>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_CONFIGM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifests;
        }

        public Manifest Create(Manifest manifest)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_CONFIGM_MISSING_APIURL);
                }
                
               var uriBuilder = new UriBuilder(ApiUrl + "/manifests");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var strManifest = ModelManager.ModelToJson(manifest);

                var json = Rest.Post(uriBuilder.Uri, strManifest);

                manifest = ModelManager.JsonToModel<Manifest>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_CONFIGM_MODEL_NOT_CREATED, manifest.name) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }

        public Manifest Update(Manifest manifest)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_CONFIGM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/manifests");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var strManifest = ModelManager.ModelToJson(manifest);

                var json = Rest.Put(uriBuilder.Uri, strManifest);

                manifest = ModelManager.JsonToModel<Manifest>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_CONFIGM_MODEL_NOT_UPDATED, manifest.name) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return manifest;
        }

        public void Delete(string id)
        {
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

                Rest.Delete(uriBuilder.Uri);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_CONFIGM_MODEL_NOT_DELETED, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }
        }
    }
}
