using System.Threading;
using BiometricsDashboard.Hubs;
using Models.Messages;

namespace BiometricsDashboard.Monitors
{
    public class GlucoseMonitor : BaseMonitor<GlucoseMessage>
    {  
        public GlucoseMonitor(HealthHubManager hubManager)
        {
            this.VitalMeasure = "glucose";
            this.UpperThreshold = 180;
            this.LowerThreshold = 100;
            this.MaxValue = 210;
            this.MinValue = 70;
            _hubManager = hubManager;
            this.Geo = "all";
            Thread monitorThread = new Thread(MonitorThreadProc);
            monitorThread.Start(new GlucoseMessage());
        }

        private void MonitorThreadProc(object state)
        {
            GlucoseMessage message = (GlucoseMessage)state;
            if (message == null) message = (GlucoseMessage)this.CreateMessage();

            while (true)
            {
                message = this.UpdateMessage(message);
                this.DumpMessage(message);

                if (_hubManager == null) continue;
                _hubManager.UpdateGlucoseMessage(message);
                //store the data so it's accessible by summary and city monitors
                if (this.Readings != null)
                {
                    _hubManager.GlucoseData = Readings;
                }
                Thread.Sleep(_hubManager.GlucoseMonitorSleepTime);
            }
        }   
         
    }
}