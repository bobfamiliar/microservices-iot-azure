using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Ref.Public.Interface;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Common.Store;

namespace LooksFamiliar.Microservices.Ref.Public.Service
{
    public class RefM : IRefPublic
    {
        private readonly IPersist _dac;

        public RefM(string docdburi, string docdbkey, string redisuri)
        {
            _dac = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            _dac.Connect("RefM", "ReferenceCollection");
        }

        public List<Entity> GetAllByDomain(string domain)
        {
            List<Entity> entityList = null;

            try
            {
                var query = "SELECT * FROM Entity e WHERE e.domain='" + domain + "'";
                entityList = _dac.SelectByQuery<Entity>(query);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_REFM_BADREQUEST, err);
            }

            return entityList;
        }

        public Entity GetByCode(string code)
        {
            Entity entity = null;

            try
            {
                var query = "SELECT * FROM Entity e WHERE e.code='" + code + "'";
                var entityList =  _dac.SelectByQuery<Entity>(query);
                if (entityList.Count > 0)
                    entity = entityList[0];
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_REFM_BADREQUEST, err);
            }

            return entity;
        }

        public List<Entity> GetByCodeValue(string codevalue)
        {
            List<Entity> entityList = null;

            try
            {
                var query = "SELECT * FROM Entity e WHERE e.codevalue='" + codevalue + "'";
                entityList = _dac.SelectByQuery<Entity>(query);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_REFM_BADREQUEST, err);
            }

            return entityList;
        }

        public List<Entity> GetAllByLink(string link)
        {
            List<Entity> entityList = null;

            try
            {
                var query = "SELECT * FROM Entity e WHERE e.link='" + link + "'";
                entityList = _dac.SelectByQuery<Entity>(query);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_REFM_BADREQUEST, err);
            }

            return entityList;
        }
    }
}