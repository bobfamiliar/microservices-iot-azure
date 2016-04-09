using System.Collections.Generic;

namespace LooksFamiliar.Microservices.Common.Store
{
    public class Persist : IPersist
    {
        private IDbase _dbase;
        //private ICache _cache;

        public Persist( IDbase dbase, ICache cache )
        {
            this._dbase = dbase;
            //this._cache = cache;
        }

        // connect to the store and the cache
        public void Connect(string databaseId, string collectionId)
        {
            _dbase.Connect(databaseId, collectionId);
            //_cache.Connect();
        }

        // select list from store
        public List<T> SelectAll<T>()
        {
            return _dbase.SelectAll<T>();
        }

        // select list from store
        public List<T> SelectByQuery<T>(string query)
        {
            return _dbase.SelectByQuery<T>(query);
        }

        // return from cache or select from store
        public T SelectById<T>(string id)
        {
            T model;

            //var objStr = _cache.Select(id);

            //if (objStr == null)
            //{
               // var objType = typeof(T);
                //var prop = objType.GetProperty("cachettl");

                model = _dbase.SelectById<T>(id);

                //if (model != null)
                //{
                //    objStr = ModelManager.ModelToJson(model);
                //    var cachettl = (int)prop.GetValue(model);
                //    _cache.Insert(id, objStr, cachettl);
                //}
            //}
            //else
            //{
            //    model = ModelManager.JsonToModel<T>(objStr);
            //}

            return model;
        }

        // Select by name and then cache the result
        public T SelectByName<T>(string name)
        {
            //var objType = typeof(T);

           // var prop1 = objType.GetProperty("id");
            //var prop2 = objType.GetProperty("cachettl");

            T model = _dbase.SelectByName<T>(name);

            //var id = (string) prop1.GetValue(model);
            //var cachettl = (int) prop2.GetValue(model);

            //var objStr = ModelManager.ModelToJson<T>(model);

            //_cache.Insert(id, objStr, cachettl);
            
            return model;
        }

        // Insert and then cache
        public void Insert<T>(T model)
        {
            _dbase.Insert<T>(model);

            //var objType = typeof(T);

            //var prop1 = objType.GetProperty("id");
            //var prop2 = objType.GetProperty("cachettl");

            //var id = (string)prop1.GetValue(model);
            //var cachettl = (int)prop2.GetValue(model);

            //var objStr = ModelManager.ModelToJson<T>(model);

            //_cache.Insert(id, objStr, cachettl);
        }

        // used for caching model lists
        //public void InsertCache<T>(T model)
        //{
        //    var objType = typeof(T);

        //    var prop1 = objType.GetProperty("id");
        //    var prop2 = objType.GetProperty("cachettl");

        //    var id = (string)prop1.GetValue(model);
        //    var cachettl = (int)prop2.GetValue(model);

        //    var objStr = ModelManager.ModelToJson<T>(model);

        //    _cache.Insert(id, objStr, cachettl);
        //}

        // Update, delete from cache and insert to cache
        public void Update<T>(T model)
        {
            _dbase.Update<T>(model);

            //var objType = typeof(T);

            //var prop1 = objType.GetProperty("id");
            //var prop2 = objType.GetProperty("cachettl");

            //var id = (string)prop1.GetValue(model);
            //var cachettl = (int)prop2.GetValue(model);

            //var objStr = ModelManager.ModelToJson<T>(model);

            //_cache.Delete(id);
            //_cache.Insert(id, objStr, cachettl);
        }

        // Delete from store and delete from cache
        public void Delete(string key)
        {
            _dbase.Delete(key);
            //_cache.Delete(key);
        }

        public List<T> SelectByModelId<T>(string modelid)
        {
            var models = _dbase.SelectByModelId<T>(modelid);
            return models;
        }
    }
}
