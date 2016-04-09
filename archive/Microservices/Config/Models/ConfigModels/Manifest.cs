using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LooksFamiliar.Microservices.Config.Models
{
    public static class LineitemsKey
    {
        public const string ModelName = "ModelName";
        public const string Database = "Database";
        public const string Collection = "Collection";
        public const string AdminAPI = "AdminAPI";
        public const string PublicAPI = "PublicAPI";
    }

    public class Lineitem
    {
        public Lineitem()
        {
            key = string.Empty;
            val = string.Empty;
        }

        public Lineitem(string _key, string _val)
        {
            key = _key;
            val = _val;
        }

        public string key { get; set; }
        public string val { get; set; }
    }

    public class Lineitems : List<Lineitem> 
    {
        public string this[string key]
        {
            get
            {
                string val = null;
                for (var i = 0; i < Count; i++)
                {
                    if (this[i].key != key) continue;
                    val = this[i].val;
                    break;
                }
                return val;
            }
            set
            {
                int i;
                var matched = false;
                for (i = 0; i < Count; i++)
                {
                    matched = (this[i].key == key);
                    if (matched) break;
                }
                if (matched)
                    this[i].val = value;
                else
                    Add(new Lineitem(key, value));
            }
        }
    }

    public class ModelBase
    {
        public ModelBase()
        {
            id = Guid.NewGuid().ToString();
            cachettl = 10;
        }

        public ModelBase(int _cachettl)
        {
            id = Guid.NewGuid().ToString();
            cachettl = _cachettl;
        }

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public int cachettl { get; set; }
    }

    public class Manifest : ModelBase
    {
        public Manifest()
        {
            modified = DateTime.Now;
            version = "1.0.0.0";
            lineitems = new Lineitems();
        }

        public DateTime modified { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public Lineitems lineitems { get; set; }

        public bool isValid()
        {
            return ((id != string.Empty) && (name != string.Empty));
        }
    }

    public class Manifests : ModelBase
    {
        public Manifests()
        {
            id = Guid.NewGuid().ToString();
            cachettl = 10;
            list = new List<Manifest>();
        }

        public List<Manifest> list { get; set; }
    }
}
