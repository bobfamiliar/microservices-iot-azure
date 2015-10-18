using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    [HealthMessage("updateSummary", "Summary")]
    public class SummaryMessage:  BaseMessage
    {
        public int GlucoseActive { get; set; }
        public int OxygenActive { get; set; }
        public int HeartRateActive { get; set; }
        public int TemperatureActive { get; set; } 
    }
}
