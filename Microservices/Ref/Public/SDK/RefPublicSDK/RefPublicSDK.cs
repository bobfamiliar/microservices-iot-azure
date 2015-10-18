using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Ref.Public.Interface;

namespace LooksFamiliar.Microservices.Ref.Public.SDK
{
    public class RefM : IRefPublic
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public RefM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public List<Entity> GetAllByDomain(string domain)
        {
            List<Entity> entityList;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities/domain/" + domain);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                entityList = ModelManager.JsonToModel<List<Entity>>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_REFM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entityList;
        }

        public Entity GetByCode(string code)
        {
            Entity entity;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities/code/" + code);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                entity = ModelManager.JsonToModel<Entity>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_REFM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entity;            
        }

        public List<Entity> GetByCodeValue(string codevalue)
        {
            List<Entity> entityList;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities/codevalue/" + codevalue);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                entityList = ModelManager.JsonToModel<List<Entity>>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_REFM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entityList;
        }

        public List<Entity> GetAllByLink(string link)
        {
            List<Entity> entityList;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities/link/" + link);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                entityList = ModelManager.JsonToModel<List<Entity>>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_REFM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entityList;
        }
    }
}
