using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Looksfamiliar.d2c2d.MessageModels
{
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

    public class DeviceProperty
    {
        public DeviceProperty()
        {
            key = string.Empty;
            val = string.Empty;
        }

        public DeviceProperty(string _key, string _val)
        {
            key = _key;
            val = _val;
        }

        public string key { get; set; }
        public string val { get; set; }
    }

    public class DeviceProperties : List<DeviceProperty>
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
                    Add(new DeviceProperty(key, value));
            }
        }
    }

    public class DeviceManifest : ModelBase
    {
        public DeviceManifest()
        {
            longitude = 0.0;
            latitude = 0.0;
            serialnumber = string.Empty;
            manufacturer = string.Empty;
            model = string.Empty;
            version = string.Empty;
            firmwareversion = string.Empty;
            hub = string.Empty;
            key = string.Empty;
            properties = new DeviceProperties();
        }

        public double longitude { get; set; }
        public double latitude { get; set; }
        public string serialnumber { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public string version { get; set; }
        public string firmwareversion { get; set; }
        public string hub { get; set; }
        public string key { get; set; }
        public DeviceProperties properties { get; set; }

        public bool isValid()
        {
            return ((serialnumber != string.Empty) &&
                    (manufacturer != string.Empty) &&
                    (model != string.Empty) &&
                    (version != string.Empty) &&
                    (firmwareversion != string.Empty));
        }
    }

    public class DeviceManifests : ModelBase
    {
        public DeviceManifests()
        {
            list = new List<DeviceManifest>();
        }

        public List<DeviceManifest> list { get; set; }
    }
}
