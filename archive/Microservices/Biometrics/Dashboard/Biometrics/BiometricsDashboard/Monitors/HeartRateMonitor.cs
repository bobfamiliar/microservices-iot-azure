using Models.Messages;
using System.Threading;
using BiometricsDashboard.Hubs;

namespace BiometricsDashboard.Monitors
{
    public class HeartRateMonitor : BaseMonitor<HeartRateMessage>
    { 
        public HeartRateMonitor(HealthHubManager hubManager)
        {
            this.VitalMeasure = "heartrate";
            this.UpperThreshold = 170;
            this.LowerThreshold = 65;
            this.MaxValue = 180;
            this.MinValue = 60;
            _hubManager = hubManager;
            this.Geo = "all";

            Thread monitorThread = new Thread(MonitorThreadProc);
            monitorThread.Start(new HeartRateMessage());
        }

        private void MonitorThreadProc(object state)
        {
            HeartRateMessage message = (HeartRateMessage)state;
            if (message == null) message = (HeartRateMessage)this.CreateMessage();

            while (true)
            {
                message = this.UpdateMessage(message);
                this.DumpMessage(message);

                if (_hubManager == null) continue;
                _hubManager.UpdateHeartRateMessage(message);
                //store the data so it's accessible by summary and city monitors
                if (this.Readings != null)
                {
                    _hubManager.HeartRateData = Readings;
                }
                Thread.Sleep(_hubManager.HeartRateMonitorSleepTime);
            }
        }
    }
}