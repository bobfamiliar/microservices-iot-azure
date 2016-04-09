using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    [HealthMessage("updateGlucose", "Glucose")]
    public class GlucoseMessage : BaseMessage
    {
        public override void Dump()
        {
            Debug.WriteLine("GlucoseMessage: {0} {1}", MessageDateTime, Status);
        }
    }
}
