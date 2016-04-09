using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BiometricsDashboard.ConfigSections
{
    public class ThreadConfigSection : ConfigurationSection
    {
        private ThreadConfigSection() { }

        #region Monitors

        [ConfigurationProperty("SummaryMonitorSleepTime", DefaultValue = "5000", IsRequired = false)]
        public int SummaryMonitorSleepTime
        {
            get { return (int)this["SummaryMonitorSleepTime"]; }
            set { this["SummaryMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("TemperatureMonitorSleepTime", DefaultValue = "2000", IsRequired = false)]
        public int TemperatureMonitorSleepTime
        {
            get { return (int)this["TemperatureMonitorSleepTime"]; }
            set { this["TemperatureMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("HeartRateMonitorSleepTime", DefaultValue = "3000", IsRequired = false)]
        public int HeartRateMonitorSleepTime
        {
            get { return (int)this["HeartRateMonitorSleepTime"]; }
            set { this["HeartRateMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("OxygenMonitorSleepTime", DefaultValue = "4000", IsRequired = false)]
        public int OxygenMonitorSleepTime
        {
            get { return (int)this["OxygenMonitorSleepTime"]; }
            set { this["OxygenMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("GlucoseMonitorSleepTime", DefaultValue = "2500", IsRequired = false)]
        public int GlucoseMonitorSleepTime
        {
            get { return (int)this["GlucoseMonitorSleepTime"]; }
            set { this["GlucoseMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("BostonMonitorSleepTime", DefaultValue = "3500", IsRequired = false)]
        public int BostonMonitorSleepTime
        {
            get { return (int)this["BostonMonitorSleepTime"]; }
            set { this["BostonMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("ChicagoMonitorSleepTime", DefaultValue = "4000", IsRequired = false)]
        public int ChicagoMonitorSleepTime
        {
            get { return (int)this["ChicagoMonitorSleepTime"]; }
            set { this["ChicagoMonitorSleepTime"] = value; }
        }

        [ConfigurationProperty("NewYorkMonitorSleepTime", DefaultValue = "4500", IsRequired = false)]
        public int NewYorkMonitorSleepTime
        {
            get { return (int)this["NewYorkMonitorSleepTime"]; }
            set { this["NewYorkMonitorSleepTime"] = value; }
        } 
        #endregion
    }
}