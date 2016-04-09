using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LooksFamiliar.Microservices.Profile.Models
{
    public static class UserType
    {
        public const string Customer = "Customer";
        public const string Employee = "Employee";
        public const string Contractor = "Contractor";
        public const string Temporary = "Temporary";
        public const string Partner = "Partner";
        public const string Participant = "Participant";
    }

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

    public class Address
    {
        public Address()
        {
            this.address1 = string.Empty;
            this.address2 = string.Empty;
            this.address3 = string.Empty;
            this.city = string.Empty;
            this.state = string.Empty;
            this.zip = string.Empty;
            this.country = string.Empty;
        }

        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
    }

    public class Social
    {
        public Social()
        {
            this.phone = string.Empty;
            this.email = string.Empty;
            this.facebook = string.Empty;
            this.twitter = string.Empty;
            this.linkedin = string.Empty;
            this.blog = string.Empty;
        }

        public string phone { get; set; }
        public string email { get; set; }
        public string linkedin { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string blog { get; set; }
    }

    public class Geo
    {
        public Geo()
        {
            longitude = 0.0;
            latitude = 0.0;
        }

        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class Preference
    {
        public Preference()
        {
            key = string.Empty;
            val = string.Empty;
        }

        public Preference(string _key, string _val)
        {
            key = _key;
            val = _val;
        }

        public string key { get; set; }
        public string val { get; set; }
    }

    public class PreferenceList : List<Preference>
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
                    Add(new Preference(key, value));
            }
        }
    }

    public class UserProfile : ModelBase
    {
        public UserProfile()
            : base(5)
        {
            firstname = string.Empty;
            lastname = string.Empty;
            username = string.Empty;
            type = string.Empty;
            address = new Address();
            social = new Social();
            preferences = new PreferenceList();
            location = new Geo();
        }

        public UserProfile(string firstname, string lastname, string username, string type)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.username = username;
            this.type = type;
            this.address = new Address();
            this.social = new Social();
            this.preferences = new PreferenceList();
            location = new Geo();
        }

        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string type { get; set; }
        public Address address { get; set; }
        public Social social { get; set; }
        public PreferenceList preferences { get; set; }
        public Geo location { get; set; }
    }

    public class UserProfiles : ModelBase
    {
        public UserProfiles()
        {
            list = new List<UserProfile>();
        }

        public List<UserProfile> list { get; set; }
    }
}
