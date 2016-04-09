using System;
using System.Collections.Generic;

namespace LooksFamiliar.Simulators.BioMax.IoTHub
{
    public enum SensorType { NotSet = 0, Glucose = 1, Heartrate = 2, Bloodoxygen = 3, Temperature = 4}

    public class Location
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class SensorReading
    {
        public SensorType type { get; set; }
        public double value { get; set; }
    }

    public class SensorReadings : List<SensorReading>
    {
    }

    public class DeviceMessage
    {
        public DeviceMessage()
        {
            reading = new DateTime();
            location = new Location();
            sensors = new SensorReadings();
        }

        public string key { get; set; }
        public string deviceid { get; set; }
        public string participantid { get; set; }
        public DateTime reading { get; set; }
        public Location location { get; set; }
        public SensorReadings sensors { get; set; }
    }
}
