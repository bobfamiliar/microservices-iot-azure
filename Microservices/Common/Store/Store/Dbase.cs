using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LooksFamiliar.Microservices.Common.Store
{
    public class Dbase : IDbase
    {
        private string _docdburi;
        private string _docdbkey;
        private DocumentClient _client;
        private Database _database;
        private DocumentCollection _collection;

        public Dbase(string docdburi, string docdbkey)
        {
            _docdburi = docdburi;
            _docdbkey = docdbkey;
        }

        public void Connect(string databaseId, string collectionId)
        {
            try
            {
                _client = new DocumentClient(new Uri(_docdburi), _docdbkey);
                GetOrCreateDatabaseAsync(databaseId).Wait();
                GetOrCreateCollectionAsync(_database.SelfLink, collectionId).Wait();
            }
            catch (Exception err)
            {
                throw new Exception(Errors.ERR_DBASE_DOCUMENTDB_CONN, err);
                ;
            }
        }

        public List<T> SelectByQuery<T>(string query)
        {
            var modelList = _client.CreateDocumentQuery<T>(_collection.SelfLink, query);
            return modelList.ToList().Select(item => (T) item).ToList();
        }

        public List<T> SelectByModelId<T>(string modelid)
        {
            var objType = typeof (T);
            var typeName = objType.Name;
            var modelList = _client.CreateDocumentQuery<T>(_collection.SelfLink,
                "SELECT * FROM " + typeName + " d WHERE d.modelid='" + modelid + "'");
            return modelList.ToList().Select(item => (T) item).ToList();
        }

        public List<T> SelectAll<T>()
        {
            var modelList = from a in _client.CreateDocumentQuery<T>(_collection.SelfLink) select a;
            return modelList.ToList().Select(item => (T) item).ToList();
        }

        public T SelectById<T>(string id)
        {
            var result = default(T);
            var objType = typeof (T);
            var typeName = objType.Name;
            var models = _client.CreateDocumentQuery<T>(_collection.SelfLink,
                "SELECT * FROM " + typeName + " d WHERE d.id='" + id + "'");
            foreach (var m in models.ToList())
            {
                result = (T) m;
            }

            return result;
        }

        public T SelectByName<T>(string name)
        {
            var result = default(T);
            var objType = typeof (T);
            var typeName = objType.Name;
            var models = _client.CreateDocumentQuery<T>(_collection.SelfLink,
                "SELECT * FROM " + typeName + " d WHERE d.name='" + name + "'");
            foreach (var m in models.ToList())
            {
                result = (T) m;
            }

            return result;
        }

        public void Insert<T>(T model)
        {
            dynamic m = _client.CreateDocumentAsync(_collection.SelfLink, model);
        }

        public void Update<T>(T model)
        {
            var objType = typeof (T);
            var id = (string) objType.GetProperty("id").GetValue(model);
            dynamic doc =
                _client.CreateDocumentQuery<Document>(_collection.SelfLink)
                    .Where(d => d.Id == id)
                    .AsEnumerable()
                    .FirstOrDefault();
            if (doc != null) _client.ReplaceDocumentAsync(doc.SelfLink, model).Wait();
        }

        // Azure DocumentDb does not suppor delete at this time
        public void Delete(string key)
        {
            //dynamic model = from a in client.CreateDocumentQuery<AppModel>(collection.SelfLink) where a.Key == key select a;
            //Document document = (Document)model;
            //client.DeleteDocumentAsync(document.SelfLink);
        }

        private async Task<Database> GetOrCreateDatabaseAsync(string id)
        {
            _database = _client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (_database != null) return _database;
            _database = await _client.CreateDatabaseAsync(new Database {Id = id});
            return _database;
        }

        private async Task<DocumentCollection> GetOrCreateCollectionAsync(string dbLink, string id)
        {
            _collection =
                _client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (_collection != null) return _collection;
            _collection = await _client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection {Id = id});
            return _collection;
        }
    }
}
