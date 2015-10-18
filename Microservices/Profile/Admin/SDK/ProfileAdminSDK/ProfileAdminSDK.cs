using System;
using LooksFamiliar.Microservices.Profile.Admin.Interface;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Common.Wire;

namespace LooksFamiliar.Microservices.Profile.Admin.SDK
{
    public class ProfileM : IProfileAdmin
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public ProfileM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }

        public void Delete(string id)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                Rest.Delete(uriBuilder.Uri);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_PROFILEM_PROFILE_NOT_DELETED, id) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }
        }

        public UserProfiles GetAll()
        {
            UserProfiles users;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users");

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                users = ModelManager.JsonToModel<UserProfiles>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_PROFILEM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return users;
        }

        public UserProfiles GetAll(string id)
        {
            UserProfiles users;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users/id/" + id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                users = ModelManager.JsonToModel<UserProfiles>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_PROFILEM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return users;
        }
    }
}
