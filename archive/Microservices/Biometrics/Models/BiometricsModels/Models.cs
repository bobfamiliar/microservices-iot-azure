using System;
using System.Collections.Generic;

namespace LooksFamiliar.Microservices.Biometrics.Models
{
    public enum BiometricType { NotSet = 0, Glucose = 1, Heartrate = 2, Bloodoxygen = 3, Temperature = 4 }

    public class BiometricReading
    {
        public BiometricReading()
        {
            type = BiometricType.NotSet;
        }
        public string deviceid { get; set; }
        public string participantid { get; set; }
        public DateTime reading { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public BiometricType type { get; set; }
        public double value { get; set; }
    }

    public class BiometricReadings : List<BiometricReading> { }
}
