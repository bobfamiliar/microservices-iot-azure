using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BiometricsDashboard.ConfigSections
{
    public class HubConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("UseSummaryMonitor", DefaultValue = "true", IsRequired = false)]
        public bool UseSummaryMonitor
        {
            get { return (bool)this["UseSummaryMonitor"]; }
            set { this["UseSummaryMonitor"] = value; }
        }

        [ConfigurationProperty("UseTemperatureMonitor", DefaultValue = "true", IsRequired = false)]
        public bool UseTemperatureMonitor
        {
            get { return (bool)this["UseTemperatureMonitor"]; }
            set { this["UseTemperatureMonitor"] = value; }
        }

        public bool UseHeartRateMonitor
        {
            get { return (bool)this["UseHeartRateMonitor"]; }
            set { this["UseHeartRateMonitor"] = value; }
        }

        public bool UseOxygenMonitor
        {
            get { return (bool)this["UseOxygenMonitor"]; }
            set { this["UseOxygenMonitor"] = value; }
        }

        public bool UseGlucoseMonitor
        {
            get { return (bool)this["UseGlucoseMonitor"]; }
            set { this["UseGlucoseMonitor"] = value; }
        }

        public bool UseBostonMonitor
        {
            get { return (bool)this["UseBostonMonitor"]; }
            set { this["UseBostonMonitor"] = value; }
        }

        public bool UseChicagoMonitor
        {
            get { return (bool)this["UseChicagoMonitor"]; }
            set { this["UseChicagoMonitor"] = value; }
        }

        public bool UseNewYorkMonitor
        {
            get { return (bool)this["UseNewYorkMonitor"]; }
            set { this["UseNewYorkMonitor"] = value; }
        } 
    }
}