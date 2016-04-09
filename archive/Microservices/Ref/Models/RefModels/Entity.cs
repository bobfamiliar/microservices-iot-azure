using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LooksFamiliar.Microservices.Ref.Models
{
    public enum SequenceType
    {
        ALPHA_ASC = 0,
        ALPHA_DEC = 1,
        FIRST = 2,
        LAST = 3
    };

    public class ModelBase
    {
        public ModelBase()
        {
            id = Guid.NewGuid().ToString();
            cachettl = 1;
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

    public class Attribute
    {
        public Attribute()
        {
            key = string.Empty;
            val = string.Empty;
        }

        public Attribute(string _key, string _val)
        {
            key = _key;
            val = _val;
        }

        public string key { get; set; }
        public string val { get; set; }
    }

    public class AttributeList : List<Attribute>
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
                    Add(new Attribute(key, value));
            }
        }
    }

    public class Entity : ModelBase
    {
        public Entity()
        {
            this.domain = string.Empty;
            this.code = string.Empty;
            this.codevalue = string.Empty;
            this.link = string.Empty;
            this.sequence = SequenceType.ALPHA_ASC;
            this.attributes = new AttributeList();
        }

        public Entity(string domain)
        {
            this.domain = domain;
            this.code = string.Empty;
            this.codevalue = string.Empty;
            this.link = string.Empty;
            this.sequence = SequenceType.ALPHA_ASC;
            this.attributes = new AttributeList();
        }

        public Entity(string domain, string code, string codeValue, SequenceType sequence)
        {
            this.domain = domain;
            this.code = code;
            this.codevalue = codeValue;
            this.link = string.Empty;
            this.sequence = sequence;
            this.attributes = new AttributeList();
        }

        public string domain { get; set; }
        public string code { get; set; }
        public string codevalue { get; set; }
        public string link { get; set; }
        public SequenceType sequence { get; set; }
        public AttributeList attributes { get; set; }
        public SequenceType SequenceType { get; set; }

        public bool isValid()
        {
            return ((id != string.Empty) && (domain != string.Empty) && (code != string.Empty) && (codevalue != string.Empty));
        }
    }

    public class Entities : ModelBase
    {
        public Entities()
        {
            id = Guid.NewGuid().ToString();
            cachettl = 10;
            list = new List<Entity>();
        }

        public List<Entity> list { get; set; }
    }
}
