using System.Collections.Generic;

namespace LooksFamiliar.Microservices.Common.Store
{
    public interface IDbase
    {
        void Connect(string databaseId, string collectionId);
        List<T> SelectAll<T>();
        List<T> SelectByQuery<T>(string query);
        List<T> SelectByModelId<T>(string modelid);
        T SelectById<T>(string id);
        T SelectByName<T>(string name);
        void Insert<T>(T model);
        void Update<T>(T model);
        void Delete(string key);
    }
}
