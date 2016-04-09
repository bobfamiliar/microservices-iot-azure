using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enums
{
    public enum GeoFilter
    {
        All,
        Boston,
        NewYork,
        Chicago
    }

    public static class GeoFilterExtension
    {
        public static String Text(this GeoFilter operation)
        {
              switch(operation)
              {
                    case GeoFilter.All: return "All";
                    case GeoFilter.Boston: return "Boston";
                    case GeoFilter.Chicago: return "Chicago";
                    case GeoFilter.NewYork: return "New York";
                  default:   return "All";
              }
        }
    }
}
