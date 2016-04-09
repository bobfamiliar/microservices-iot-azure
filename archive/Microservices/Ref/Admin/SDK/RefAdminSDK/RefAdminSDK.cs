using System;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Ref.Admin.Interface;

namespace LooksFamiliar.Microservices.Ref.Admin.SDK
{
    public class RefM : IRefAdmin
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public RefM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public Entities GetAll()
        {
            Entities entities;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                entities = ModelManager.JsonToModel<Entities>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_REFM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entities;
        }

        public Entities GetAll(string id)
        {
            Entities entities;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                entities = ModelManager.JsonToModel<Entities>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_REFM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entities;
        }


        public Entity Create(Entity entity)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var strModel = ModelManager.ModelToJson(entity);

                var json = Rest.Post(uriBuilder.Uri, strModel);

                entity = ModelManager.JsonToModel<Entity>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_REFM_ENTITY_NOT_CREATED, entity.code) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entity;
        }

        public Entity Update(Entity entityModel)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var strModel = ModelManager.ModelToJson(entityModel);

                var json = Rest.Put(uriBuilder.Uri, strModel);

                entityModel = ModelManager.JsonToModel<Entity>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_REFM_ENTITY_NOT_UPDATED, entityModel.code) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return entityModel;
        }

        public void Delete(string id)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_REFM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/entities/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                Rest.Delete(uriBuilder.Uri);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_REFM_ENTITY_NOT_DELETED, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }
        }
    }
}
