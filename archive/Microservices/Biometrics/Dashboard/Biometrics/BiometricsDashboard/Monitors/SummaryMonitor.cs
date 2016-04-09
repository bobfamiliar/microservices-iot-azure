using Models.Entities;
using Models.Messages;
using System;
using System.Linq;
using System.Threading;
using BiometricsDashboard.Hubs;
using Models.Enums;

namespace BiometricsDashboard.Monitors
{
    public class SummaryMonitor: BaseMonitor<SummaryMessage>
    {
        #region Constructors

        public SummaryMonitor(HealthHubManager hubManager)
        {
            _hubManager = hubManager;
            this.Geo = "all";

            Thread monitorThread = new Thread(MonitorThreadProc);
            monitorThread.Start(new SummaryMessage());
        }

        #endregion 

        #region Methods 

        private void SummarizeData(SummaryMessage msg)
        {
            msg.TotalCount = 300; 
            var countHigh = _hubManager.GetCountByState(Condition.High);
            var countNormal = _hubManager.GetCountByState(Condition.Normal);
            var countLow = _hubManager.GetCountByState(Condition.Low); 

            //aggregate by condition
            msg.HighPercent = Convert.ToInt32(countHigh * 100 / 4 / msg.TotalCount);
            msg.NormalPercent = Convert.ToInt32(countNormal * 100 / 4 / msg.TotalCount);
            msg.LowPercent = Convert.ToInt32(countLow * 100 / 4 / msg.TotalCount);
            msg.InactivePercent = 100 - msg.HighPercent - msg.NormalPercent - msg.LowPercent;

            //active vs inactive
            msg.GlucoseActive = _hubManager.GlucoseData?.Count() ?? 0;
            msg.TemperatureActive = _hubManager.TempData?.Count() ?? 0;
            msg.OxygenActive = _hubManager.OxygenData?.Count() ?? 0;
            msg.HeartRateActive = _hubManager.HeartRateData?.Count() ?? 0;
             
            msg.Distribution.Clear();
            
            msg.Distribution.Add(new DistributionItem() { Label = GeoFilter.Boston.Text(), Count = _hubManager.GetCountByCity(GeoFilter.Boston) });
            msg.Distribution.Add(new DistributionItem() { Label = GeoFilter.Chicago.Text(), Count = _hubManager.GetCountByCity(GeoFilter.Chicago) });
            msg.Distribution.Add(new DistributionItem() { Label = GeoFilter.NewYork.Text(), Count = _hubManager.GetCountByCity(GeoFilter.NewYork) }); 
        }   

        private void MonitorThreadProc(object state)
        {
            SummaryMessage message = (SummaryMessage)state;
            if (message == null) message = this.CreateMessage();

            while (true)
            {
                message = this.UpdateMessage(message);
                this.DumpMessage(message);

                if (_hubManager == null) continue;
                _hubManager.UpdateSummaryMessage(message);

                Thread.Sleep(_hubManager.SummaryMonitorSleepTime);
            }
        }

        private SummaryMessage UpdateMessage(SummaryMessage msg)
        {
            if (this.ContextChanged(msg))
            {
                msg = CreateMessage();
            }
            this.SummarizeData(msg); 
            return msg;
        }

        private bool ContextChanged(SummaryMessage msg)
        {
            var latestContext = _hubManager.LatestContextMessage;
            if (msg.GeoFilter != latestContext.GeoFilter)
                return true;
            else
                return false;
        }

        #endregion 
    }
}