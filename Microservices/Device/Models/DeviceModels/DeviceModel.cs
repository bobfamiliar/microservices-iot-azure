using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LooksFamiliar.Microservices.Device.Models
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

    public class Registration : ModelBase
    {
        public Registration()
        {
            key = string.Empty;
            participantid = string.Empty;
            productline = string.Empty;
            model = string.Empty;
            version = string.Empty;
            firmwareversion = string.Empty;
        }

        public string key { get; set; }
        public string participantid { get; set; }
        public string productline { get; set; }
        public string model { get; set; }
        public string version { get; set; }
        public string firmwareversion { get; set; }

        public bool isValid()
        {
            return ((productline != string.Empty) && 
                    (model != string.Empty) && 
                    (version != string.Empty) && 
                    (firmwareversion != string.Empty));
        }
    }

    public class Registrations : ModelBase
    {
        public Registrations()
        {
            list = new List<Registration>();
        }

        public List<Registration> list { get; set; }
    }
}
