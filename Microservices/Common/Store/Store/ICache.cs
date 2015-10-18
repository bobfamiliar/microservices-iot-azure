namespace LooksFamiliar.Microservices.Common.Store
{
    public interface ICache
    {
        void Connect();
        void Insert(string key, string value, int ttl);
        bool Exists(string key);
        void Update(string key, string value, int ttl);
        string Select(string key);
        void Delete(string key);
        void Clear();
    }
}
