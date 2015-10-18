using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Profile.Admin.Interface;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Common.Store;

namespace LooksFamiliar.Microservices.Profile.Admin.Service
{
    public class ProfileM : IProfileAdmin
    {
        private readonly IPersist _persist;

        public ProfileM(string docdburi, string docdbkey, string redisuri)
        {
            _persist = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            _persist.Connect("ProfileM", "ProfileCollection");
        }

        public UserProfiles GetAll()
        {
            UserProfiles users = new UserProfiles();

            try
            {
                List<UserProfile> userList = _persist.SelectAll<UserProfile>();
                foreach( UserProfile u in userList)
                {
                    users.list.Add(u);
                }
                this._persist.InsertCache<UserProfiles>(users);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_PROFILES_NOT_FOUND, err);
            }

            return users;
        }

        public UserProfiles GetAll(string id)
        {
            UserProfiles users = null;

            try
            {
                // try to get from cache
                users = this._persist.SelectById<UserProfiles>(id);
                if (users == null)
                    return GetAll();
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_PROFILES_NOT_FOUND, err);
            }

            return users;
        }

        public void Delete(string id)
        {
            try
            {
                _persist.Delete(id);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}