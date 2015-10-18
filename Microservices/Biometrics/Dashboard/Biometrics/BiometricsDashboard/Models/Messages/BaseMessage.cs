using Models.Entities;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    public class BaseMessage
    {
        public BaseMessage()
        {
            MessageDateTime = DateTime.Now;
            HighPercent = 0;
            TotalCount = 0;
            NormalPercent = 0;
            LowPercent = 0;
            InactivePercent = 0;
            Distribution = new List<DistributionItem>();
        }

        public virtual void Dump() { }
        //msg members
        public GeoFilter GeoFilter { get; set; } 
        public DateTime MessageDateTime { get; set; }
        public DateTime TimeFilter { get; set; }
        public string Status { get; set; }
        public bool IsDefault { get; set; }

        //health members
        public int TotalCount { get; set; }
        public int HighPercent { get; set; }
        public int NormalPercent { get; set; }
        public int LowPercent { get; set; }
        public int InactivePercent { get; set; }
        public List<DistributionItem> Distribution { get; set; }
        public List<Location> Locations { get; set; }
        public double AverageReading { get; set; }
        public double MaxReading { get; set; }
        public double MinReading { get; set; }

    }
}
