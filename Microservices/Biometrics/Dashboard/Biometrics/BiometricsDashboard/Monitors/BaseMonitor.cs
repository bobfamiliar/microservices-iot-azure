using Models.Entities;
using Models.Enums;
using Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BiometricsDashboard.Hubs;
using LooksFamiliar.Microservices.Biometrics.Models;

namespace BiometricsDashboard.Monitors
{
    public abstract class BaseMonitor<T> where T: BaseMessage
    {
        public string api { get; set; }
        public string VitalMeasure { get; set; }
        public string Geo { get; set; }
        public double UpperThreshold { get; set; }
        public double LowerThreshold { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public List<Measurement> Readings { get; set; }
        protected virtual List<DistributionItem> SetGeoCounts(List<Measurement> data)
        {
            var counts = new List<DistributionItem>();
            var hi = data.Where(r => r.State == Condition.High);
            var normal = data.Where(r => r.State == Condition.Normal);
            var low = data.Where(r => r.State == Condition.Low);
            counts.Add(new DistributionItem() { Label = "High", Count = hi != null ? hi.Count() : 0 });
            counts.Add(new DistributionItem() { Label = "Normal", Count = normal != null ? normal.Count() : 0 });
            counts.Add(new DistributionItem() { Label = "Low", Count = low    != null ? low.Count() : 0 });
            return counts;
        } 

        public T CreateMessage() 
        {
            var message = new BaseMessage {GeoFilter = _hubManager.LatestContextMessage.GeoFilter};
            return (T)message;
        } 

        protected List<Measurement> GetData(string _measure = null)
        {
            var factory = new BiometricReadingsFactory();
            var url = $"{api}/city/{Geo}/type/{VitalMeasure}/count/{10}";

            BiometricReadings msg = null;

            try
            {
                msg = factory.GetBiometrics(url);
            }
            catch (Exception err)
            {
                Trace.TraceError(err.Message);
            }

            var list = new List<Measurement>();

            foreach (BiometricReading vital in msg)
            {
                var item = new Measurement() { Reading = vital.value };
                if (item.Reading > this.UpperThreshold)
                {
                    item.State = Condition.High;
                }
                else if (item.Reading < this.LowerThreshold)
                {
                    item.State = Condition.Low;
                }
                else
                {
                    item.State = Condition.Normal;
                }
                item.Person = new Person(vital.latitude, vital.longitude);

                list.Add(item);
            }
            return list;
        } 

        protected virtual T UpdateMessage(T msg)  
        {
            this.Readings = GetData(this.VitalMeasure);
            msg.TotalCount = _hubManager.GetPatientCount();
            if (this.Readings.Count() > 0 && msg.TotalCount > 0)
            {
                if (msg.GeoFilter != GeoFilter.All)
                {
                    Readings = Readings.Where(t => t.Person.Location == msg.GeoFilter).ToList();
                    msg.Distribution = new List<DistributionItem>();
                }
                else
                {
                    msg.Distribution = SetGeoCounts(Readings);
                }
                msg.AverageReading = Readings.Sum(t => t.Reading) / Readings.Count();
                msg.MaxReading = this.MaxValue;
                msg.MinReading = this.MinValue;
                msg.HighPercent = Convert.ToInt32(Readings.Where(t => t.State == Condition.High).Count() * (100.0 / msg.TotalCount));
                msg.NormalPercent = Convert.ToInt32(Readings.Where(t => t.State == Condition.Normal).Count() * (100.0 / msg.TotalCount));
                msg.LowPercent = Convert.ToInt32(Readings.Where(t => t.State == Condition.Low).Count() * (100.0 / msg.TotalCount));
                msg.InactivePercent = 100 - msg.HighPercent - msg.NormalPercent - msg.LowPercent; 
            }
            else
            {
                msg.HighPercent = 0;
                msg.NormalPercent = 0;
                msg.LowPercent = 0;
                msg.InactivePercent = 0;
            }
            return msg;
        }

        protected void DumpMessage(T message) 
        {
            if (message == null) return;

            Debug.Write(string.Format(typeof(T).ToString() + ": {0} {1}", message.MessageDateTime, message.Status));
        }

        #region Properties

        protected HealthHubManager _hubManager;

        #endregion
    }

}