using Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Messages
{
    [HealthMessage("updateTemperature", "Temperature")]
    public class TemperatureMessage : BaseMessage
    {
        public TemperatureMessage()
        {
            var context = new ContextMessage
            {
                GeoFilter = Models.Enums.GeoFilter.All, 
                TimeFilter = DateTime.Today,
                MessageDateTime = DateTime.Now
            };
            Initialize(context);
        }

        public TemperatureMessage(ContextMessage context)
        {
            Initialize(context);
        } 


        private void Initialize(ContextMessage context)
        { 
            GeoFilter = context.GeoFilter;
            TimeFilter = context.TimeFilter;
            MessageDateTime = DateTime.Now; 
        }

        public void Dump()
        {
            Debug.WriteLine("TemperatureMessage: {0} {1}", MessageDateTime, Status); 
        }
    }
}