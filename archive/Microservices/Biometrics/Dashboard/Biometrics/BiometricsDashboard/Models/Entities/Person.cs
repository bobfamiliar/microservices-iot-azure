using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Person
    {
        public Person() { }
        public Person(double _latitude, double _longitude)
        {
            Latitude = _latitude;
            Longitude = _longitude;
            if (Longitude > 80 || Longitude < -80)
            {
                Location = GeoFilter.Chicago;
            }
            else if (Latitude > 41.2)
            {
                Location = GeoFilter.Boston;
            }
            else
            {
                Location = GeoFilter.NewYork;
            }
        }
        public string ID { get; set; }
        public string PostalCode { get; set; }
        public GeoFilter Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; } 
    }
}
