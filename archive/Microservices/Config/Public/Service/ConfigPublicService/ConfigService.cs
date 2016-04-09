using System;
using LooksFamiliar.Microservices.Config.Public.Interface;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Common.Store;

namespace LooksFamiliar.Microservices.Config.Public.Service
{
    public class ConfigM : IConfig
    {
        private readonly IPersist _persist;

        public ConfigM(string docdburi, string docdbkey, string redisuri)
        {
            this._persist = new Persist(new Dbase(docdburi, docdbkey), new Cache(redisuri));
            this._persist.Connect("ConfigM", "ManifestCollection");
        }

        public Manifest GetById(string id)
        {
            Manifest manifest = null;

            try
            {
                manifest = this._persist.SelectById<Manifest>(id);
            }
            catch (Exception err)
            {
                throw new Exception(Error.ERR_CONFIGM_BADREQUEST, err);
            }

            return manifest;
        }

        public Manifest GetByName(string name)
        {
            Manifest manifest = null;

            try
            {
                manifest = this._persist.SelectByName<Manifest>(name);
            }
            catch (Exception err)
            {
                throw new Exception(Error.ERR_CONFIGM_BADREQUEST, err);
            }

            return manifest;        
        }
    }
}