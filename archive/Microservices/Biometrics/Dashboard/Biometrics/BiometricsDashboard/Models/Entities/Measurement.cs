using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Measurement
    {
        #region Properties

        public Person Person { get; set; }
        public double Reading { get; set; }
        public Condition State { get; set; } 

        #endregion
    }
}
