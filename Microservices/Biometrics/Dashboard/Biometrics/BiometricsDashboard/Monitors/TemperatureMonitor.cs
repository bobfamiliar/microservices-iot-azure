using Models.Messages;
using System.Threading;
using BiometricsDashboard.Hubs;

namespace BiometricsDashboard.Monitors
{
    public class TemperatureMonitor : BaseMonitor<TemperatureMessage>
    {
        public TemperatureMonitor(HealthHubManager hubManager)
        {
            this.VitalMeasure = "temperature";
            this.UpperThreshold = 104;
            this.LowerThreshold = 98;
            this.MaxValue = 106;
            this.MinValue = 98;
            _hubManager = hubManager;
            this.Geo = "all";

            Thread monitorThread = new Thread(MonitorThreadProc);
            monitorThread.Start(new TemperatureMessage());
        }

        private void MonitorThreadProc(object state)
        {
            TemperatureMessage message = (TemperatureMessage)state;
            if (message == null) message = (TemperatureMessage)this.CreateMessage();

            while (true)
            {
                message = this.UpdateMessage(message);
                this.DumpMessage(message);

                if (_hubManager == null) continue;
                _hubManager.UpdateTemperatureMessage(message);

                //store the data so it's accessible by summary and city monitors
                if (this.Readings != null)
                {
                    _hubManager.TempData = Readings;
                }
                Thread.Sleep(_hubManager.TemperatureMonitorSleepTime);
            }
        }
    }
}