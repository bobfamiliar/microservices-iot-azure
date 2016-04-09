using StackExchange.Redis;
using System;
using System.Linq;

namespace LooksFamiliar.Microservices.Common.Store
{
    public class Cache : ICache
    {
        private string _redisuri;
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;

        public Cache(string redisuri)
        {
            _redisuri = redisuri;
        }
        public void Connect()
        {
            _connection = ConnectionMultiplexer.Connect(_redisuri);
            _cache = _connection.GetDatabase();
        }
        public void Insert(string key, string val, int ttl)
        {
            _cache.StringSet(key, val, TimeSpan.FromMinutes(ttl));
        }

        public void Update(string key, string val, int ttl)
        {
            Delete(key);
            Insert(key, val, ttl);
        }

        public string Select(string key)
        {
            return _cache.StringGet(key);
        }

        public bool Exists(string key)
        {
            return _cache.KeyExists(key);
        }

        public void Delete(string key)
        {
            _cache.KeyDelete(key);
        }

        public void Clear()
        {
            var endpoints = _connection.GetEndPoints(true);
            foreach (var server in endpoints.Select(endpoint => _connection.GetServer(endpoint)))
            {
                server.FlushAllDatabases();
            }
        }
    }
}
