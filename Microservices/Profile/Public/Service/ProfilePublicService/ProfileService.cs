using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Profile.Public.Interface;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Common.Store;

namespace LooksFamiliar.Microservices.Profile.Public.Service
{
    public class ProfileM : IProfile
    {
        private readonly IPersist _dac;

        public ProfileM(string docdburi, string docdbkey, string redisuri)
        {
            _dac = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            _dac.Connect("ProfileM", "ProfileCollection");
        }

        public UserProfile Create(UserProfile profile)
        {
            try
            {
                _dac.Insert(profile);
            }
            catch (Exception err)
            {
               throw new Exception(Errors.ERR_PROFILEM_BADREQUEST, err);
            }

            return profile;
        }

        public UserProfile GetById(string id)
        {
            UserProfile user = null;

            try
            {
                user = _dac.SelectById<UserProfile>(id); ;
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_BADREQUEST, err);
            }

            return user;
        }

        public UserProfile GetByName(string firstname, string lastname)
        {
            UserProfile user = null;

            try
            {
                var query = "SELECT * FROM UserProfile u WHERE u.firstname='" + firstname + "' and u.lastname='" + lastname + "'";
                var users = _dac.SelectByQuery<UserProfile>(query);

                if (users != null)
                {
                    if (users.Count > 0) user = users[0];
                }
                else
                {
                    throw new Exception(Errors.ERR_PROFILEM_PROFILE_NOT_FOUND);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_BADREQUEST, err);
            }

            return user;
        }

        public UserProfile Update(UserProfile user)
        {
            try
            {
                _dac.Update(user);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_BADREQUEST, err);
            }

            return user;
        }

        public List<UserProfile> GetAllByType(string type)
        {
            List<UserProfile> users = null;

            try
            {
                var query = "SELECT * FROM UserProfile u WHERE u.type='" + type + "'";
                users = _dac.SelectByQuery<UserProfile>(query);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_BADREQUEST, err);
            }

            return users;
        }

        public List<UserProfile> GetByState(string state)
        {
            List<UserProfile> users = null;

            try
            {
                var query = "SELECT * FROM UserProfile p where p.address.state='" + state + "'";
                users = _dac.SelectByQuery<UserProfile>(query);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_PROFILEM_BADREQUEST, err);
            }

            return users;
        }
    }
}