using Models.Messages;
using System.Threading;
using BiometricsDashboard.Hubs;

namespace BiometricsDashboard.Monitors
{
    public class OxygenMonitor : BaseMonitor<OxygenMessage>
    {
        public OxygenMonitor(HealthHubManager hubManager)
        {
            this.VitalMeasure = "bloodoxygen";
            this.UpperThreshold = 90;
            this.LowerThreshold = 85;
            this.MaxValue = 100;
            this.MinValue = 80;
            _hubManager = hubManager;
            this.Geo = "all";

            Thread monitorThread = new Thread(MonitorThreadProc);
            monitorThread.Start(new OxygenMessage());
        }

        private void MonitorThreadProc(object state)
        {
            OxygenMessage message = (OxygenMessage)state;
            if (message == null) message = (OxygenMessage)this.CreateMessage();

            while (true)
            {
                message = this.UpdateMessage(message);
                this.DumpMessage(message);
                //store the data so it's accessible by summary and city monitors
                if (this.Readings != null)
                {
                    _hubManager.OxygenData = Readings;
                }

                if (_hubManager == null) continue;
                _hubManager.UpdateOxygenMessage(message);

                Thread.Sleep(_hubManager.OxygenMonitorSleepTime);
            }
        }
    }
}