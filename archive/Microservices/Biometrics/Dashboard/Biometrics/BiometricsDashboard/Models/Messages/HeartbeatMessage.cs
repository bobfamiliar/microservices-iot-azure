using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    [HealthMessage("heartbeat", "")]
    public class HeartbeatMessage:  BaseMessage
    {
        public bool IsDefault { get; set; }
    }
}
