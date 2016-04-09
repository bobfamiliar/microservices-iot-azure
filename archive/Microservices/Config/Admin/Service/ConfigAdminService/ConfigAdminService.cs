using System;
using System.Collections.Generic;
using LooksFamiliar.Microservices.Config.Admin.Interface;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Common.Store;

namespace LooksFamiliar.Microservices.Config.Admin.Service
{
    public class ConfigM : IConfigAdmin
    {
        private readonly IPersist _persist;

        public ConfigM(string docdburi, string docdbkey, string redisuri)
        {
            this._persist = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            this._persist.Connect("ConfigM", "ManifestCollection");
        }

        public Manifest Create(Manifest manifest)
        {
            try
            {
                if (manifest.isValid())
                    this._persist.Insert(manifest);
                else
                {
                    throw new Exception(Errors.ERR_CONFIGM_MODEL_NOT_VALID);
                }
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_CONFIGM_BAGREQUEST, err);
            }

            return manifest;
        }

        public Manifest Update(Manifest model)
        {
            try
            {
                if (model.isValid())
                    this._persist.Update(model);
                else
                {
                    throw new Exception(Errors.ERR_CONFIGM_MODEL_NOT_VALID);                    
                }
            }
            catch (Exception err)
            {
                throw new Exception(string.Format(Errors.ERR_CONFIGM_MODEL_NOT_UPDATED, model.name), err);
            }

            return model;
        }

        public void Delete(string id)
        {
            try
            {
                this._persist.Delete(id);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_CONFIGM_MODEL_NOT_DELETED, err);
            }
        }

        public Manifests GetAll()
        {
            Manifests manifests = new Manifests();

            try
            {
                List<Manifest> manifestList = this._persist.SelectAll<Manifest>();
                foreach(Manifest m in manifestList)
                {
                    manifests.list.Add(m);
                }
                this._persist.InsertCache(manifests);
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_CONFIGM_BAGREQUEST, err);
            }

            return manifests;
        }

        public Manifests GetAll(string id)
        {
            Manifests manifests;

            try
            {
                // this will return the list from cache or if null, get from the db
                manifests = this._persist.SelectById<Manifests>(id);
                if (manifests == null)
                    return GetAll();
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_CONFIGM_BAGREQUEST, err);
            }

            return manifests;
        }
    }
}