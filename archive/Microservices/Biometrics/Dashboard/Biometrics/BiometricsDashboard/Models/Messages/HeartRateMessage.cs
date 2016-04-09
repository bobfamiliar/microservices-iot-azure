using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    [HealthMessage("updateHeartRate", "HeartRate")]
    public class HeartRateMessage : BaseMessage
    {
        public override void Dump()
        {
            Debug.WriteLine("HeartRateMessage: {0} {1}", MessageDateTime, Status);
        }
    }
}
