using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Vitals
    {
        public Vitals()
        {
            reading = new DateTime();
        }
        public DateTime reading { get; set; }
        public int glucose { get; set; }
        public int heartRate { get; set; }
        public double tempurature { get; set; }
        public int weight { get; set; }
        public int oxygenSaturation { get; set; }
    }

    public class VitalReading
    {
        public bool isAlarm { get; set; }
        public Location geo { get; set; }
        public DateTime reading { get; set; }
        public double value { get; set; }
    }
}
