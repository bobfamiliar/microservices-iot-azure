using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class TemperatureItem
    {
        #region Properties

        public Person Person { get; set; }
        public double Temperature { get; set; }
        public Condition State { get; set; }
        public double TemperatureChange { get; set; }
        public string TemperatureUnits { get; set; }

        #endregion
    }
}
