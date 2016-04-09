using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Profile.Public.Interface;
using LooksFamiliar.Microservices.Common.Wire;


namespace LooksFamiliar.Microservices.Profile.Public.SDK
{
    public class ProfileM : IProfile
    {
        public string DevKey { get; set; }
        public string ApiUrl { get; set; }

        public ProfileM()
        {
            DevKey = string.Empty;
            ApiUrl = string.Empty;
        }
        
        public UserProfile Create(UserProfile profile)
        {
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

                var strModel = ModelManager.ModelToJson(profile);

                var json = Rest.Post(uriBuilder.Uri, strModel);

                profile = ModelManager.JsonToModel<UserProfile>(json);

            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_PROFILEM_PROFILE_NOT_CREATED, profile.firstname + " " + profile.lastname) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return profile;            
        }


        public UserProfile Update(UserProfile user)
        {
            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users/id/" + user.id);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var strModel = ModelManager.ModelToJson(user);

                var json = Rest.Put(uriBuilder.Uri, strModel);

                user = ModelManager.JsonToModel<UserProfile>(json);
            }
            catch (Exception err)
            {
                var errString = string.Format(Errors.ERR_PROFILEM_PROFILE_NOT_CREATED, user.firstname + " " + user.lastname) + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return user;
        }

        public UserProfile GetById(string id)
        {
            UserProfile user;

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

                user = ModelManager.JsonToModel<UserProfile>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_PROFILEM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return user;
        }

        public UserProfile GetByName(string firstname, string lastname)
        {
            UserProfile user;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users/firstname/" + firstname + "/lastname/" + lastname);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                user = ModelManager.JsonToModel<UserProfile>(json);
            }
            catch (Exception err)
            {
                var errString = Errors.ERR_PROFILEM_NO_RESULTS + ", " + err.Message;
                if (err.InnerException != null)
                    errString += ", " + err.InnerException.Message;
                throw new Exception(errString);
            }

            return user;
        }

        public List<UserProfile> GetAllByType(string type)
        {
            List<UserProfile> users;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users/type/" + type);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                users = ModelManager.JsonToModel<List<UserProfile>>(json);
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

        public List<UserProfile> GetByState(string state)
        {
            List<UserProfile> users;

            try
            {
                if (ApiUrl == string.Empty)
                {
                    throw new Exception(Errors.ERR_PROFILEM_MISSING_APIURL);
                }

                var uriBuilder = new UriBuilder(ApiUrl + "/users/state/" + state);

                if (DevKey != string.Empty)
                {
                    uriBuilder.Query = "subscription-key=" + DevKey;
                }

                var json = Rest.Get(uriBuilder.Uri);

                users = ModelManager.JsonToModel<List<UserProfile>>(json);
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
