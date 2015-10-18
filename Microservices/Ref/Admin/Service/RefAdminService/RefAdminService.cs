using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Ref.Admin.Interface;
using LooksFamiliar.Microservices.Ref.Models;
using LooksFamiliar.Microservices.Common.Store;

namespace LooksFamiliar.Microservices.Ref.Admin.Service
{
    public class RefM : IRefAdmin
    {
        private readonly IPersist _persist;

        public RefM(string docdburi, string docdbkey, string redisuri)
        {
            _persist = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            _persist.Connect("RefM", "ReferenceCollection");
        }

        public Entities GetAll()
        {
            Entities entities = new Entities();

            try
            {
                List<Entity> entityList = _persist.SelectAll<Entity>();
                foreach ( Entity e in entityList)
                {
                    entities.list.Add(e);
                }
                this._persist.InsertCache(entities);
            }
            catch (Exception err)
            {
                throw new Exception(Error.ERR_REFM_BADREQUEST, err);
            }

            return entities;       
        }

        public Entities GetAll(string id)
        {
            Entities entities;

            try
            {
                // this will return the list from cache or if null, get from the db
                entities = this._persist.SelectById<Entities>(id);
                if (entities == null)
                    return GetAll();
            }
            catch (Exception err)
            {
                throw new Exception(Error.ERR_REFM_BADREQUEST, err);
            }

            return entities;
        }

        public Entity Create(Entity entity)
        {
            try
            {
                if (entity.isValid())
                    _persist.Insert(entity);
                else
                {
                    throw new Exception(Error.ERR_REFM_ENTITY_NOT_VALID);   
                }
            }
            catch (Exception err)
            {
                throw new Exception(Error.ERR_REFM_BADREQUEST, err);
            }

            return entity;
        }

        public Entity Update(Entity entity)
        {
            try
            {
                if (entity.isValid())
                {
                    _persist.Update(entity);
                }
                else
                {
                    throw new Exception(Error.ERR_REFM_ENTITY_NOT_VALID);   
                }
            }
            catch (Exception err)
            {
                throw new Exception(Error.ERR_REFM_BADREQUEST, err);
            }

            return entity;
        }

        public void Delete(string id)
        {
            try
            {
                _persist.Delete(id);
            }
            catch (Exception)
            {
                // Ignored
            }
        }
    }
}