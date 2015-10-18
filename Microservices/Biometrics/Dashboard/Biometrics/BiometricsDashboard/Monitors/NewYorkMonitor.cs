using Models.Entities;
using Models.Messages;
using System;
using System.Threading;
using BiometricsDashboard.Hubs;
using Models.Enums;

namespace BiometricsDashboard.Monitors
{
    public class NewYorkMonitor : BaseMonitor<NewYorkMessage>
    {
        public NewYorkMonitor(HealthHubManager hubManager)
        {
            this.Geo = "newyork"; 
            _hubManager = hubManager;

            Thread monitorThread = new Thread(MonitorThreadProc);
            monitorThread.Start(new NewYorkMessage());
        }

        private void MonitorThreadProc(object state)
        {
            NewYorkMessage message = (NewYorkMessage)state;
            if (message == null) message = (NewYorkMessage)this.CreateMessage();

            while (true)
            {
                this.SummarizeData(message);
                this.DumpMessage(message);

                if (_hubManager == null) continue;
                _hubManager.UpdateNewYorkMessage(message);

                Thread.Sleep(_hubManager.NewYorkMonitorSleepTime);
            }
        }

        private void SummarizeData(NewYorkMessage msg)
        {
            msg.TotalCount = 100; //800;// _hubManager.GetPatientCount();
            var highCount = _hubManager.GetCountByCityState(GeoFilter.NewYork, Condition.High);
            var normalCount = _hubManager.GetCountByCityState(GeoFilter.NewYork, Condition.Normal);
            var lowCount = _hubManager.GetCountByCityState(GeoFilter.NewYork, Condition.Low);
            var inactiveCount = msg.TotalCount - highCount - normalCount - lowCount;
            if (_hubManager.GetPatientCount() > 0)
            {
                msg.HighPercent = Convert.ToInt32(100 * highCount / msg.TotalCount);
                msg.NormalPercent = Convert.ToInt32(100 * normalCount / msg.TotalCount);
                msg.LowPercent = Convert.ToInt32(100 * lowCount / msg.TotalCount);
                msg.InactivePercent = Convert.ToInt32(100 * inactiveCount / msg.TotalCount);
                msg.Distribution.Clear();
                msg.Distribution.Add(new DistributionItem() { Label = "Inactive", Count = inactiveCount });
                msg.Distribution.Add(new DistributionItem() { Label = "Normal", Count = normalCount });
                msg.Distribution.Add(new DistributionItem() { Label = "High", Count = highCount });
                msg.Distribution.Add(new DistributionItem() { Label = "Low", Count = lowCount });
                msg.Locations = _hubManager.GetLocationsByCity(GeoFilter.NewYork);
            }
        }
    }
}