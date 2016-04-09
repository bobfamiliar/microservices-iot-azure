using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Models.Entities;
using Models.Enums;
using Models.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BiometricsDashboard.ConfigSections;
using BiometricsDashboard.Monitors;
using BiometricsDashboard.Utilities;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Public.SDK;

namespace BiometricsDashboard.Hubs
{
    public class HealthHubManager
    {
        private static readonly HealthHubManager _instance = new HealthHubManager(); 
        private volatile ContextMessage _latestContextMessage;
        //private HubConfigSection _hubConfig; 
        private volatile SummaryMessage _latestSummaryMessage;
        private volatile TemperatureMessage _latestTemperatureMessage;
        private volatile HeartRateMessage _latestHeartRateMessage;
        private volatile OxygenMessage _latestOxygenMessage;
        private volatile GlucoseMessage _latestGlucoseMessage;
        private volatile BostonMessage _latestBostonMessage;
        private volatile ChicagoMessage _latestChicagoMessage;
        private volatile NewYorkMessage _latestNewYorkMessage;
        private volatile SummaryMonitor _summaryMonitor;
        private TemperatureMonitor _temperatureMonitor;
        private HeartRateMonitor _heartRateMonitor;
        private GlucoseMonitor _glucoseMonitor;
        private OxygenMonitor _oxygenMonitor;
        private BostonMonitor _bostonMonitor;
        private ChicagoMonitor _chicagoMonitor;
        private NewYorkMonitor _newYorkMonitor;
        private ThreadConfigSection _threadConfig;
        private Dictionary<string, Action> _sendToGroup;


        private HealthHubManager()
        {
            InitializeContext();
            InitializeConfig();
            InitializeMessages();
            InitializeMonitors(); 

            Task.Run(() => HeartbeatTask());
        }

        public int SummaryMonitorSleepTime
        {
            get { return _threadConfig.SummaryMonitorSleepTime; }
        }

        public int TemperatureMonitorSleepTime
        {
            get { return _threadConfig.TemperatureMonitorSleepTime; }
        }

        public int HeartRateMonitorSleepTime
        {
            get { return _threadConfig.HeartRateMonitorSleepTime; }
        }

        public int GlucoseMonitorSleepTime
        {
            get { return _threadConfig.GlucoseMonitorSleepTime; }
        }

        public int BostonMonitorSleepTime
        {
            get { return _threadConfig.BostonMonitorSleepTime; }
        }

        public int ChicagoMonitorSleepTime
        {
            get { return _threadConfig.ChicagoMonitorSleepTime; }
        }

        public int NewYorkMonitorSleepTime
        {
            get { return _threadConfig.NewYorkMonitorSleepTime; }
        }

        public int OxygenMonitorSleepTime
        {
            get { return _threadConfig.OxygenMonitorSleepTime; }
        } 

        public static HealthHubManager Instance
        {
            get { return _instance; }
        }  

        public ContextMessage LatestContextMessage
        {
            get { return _latestContextMessage; }
        }
        public TemperatureMessage LatestTemperatureMessage
        {
            get { return _latestTemperatureMessage; }
        }
        public SummaryMessage LatestSummaryMessage
        {
            get { return _latestSummaryMessage; }
        }
        public HeartRateMessage LatestHeartRateMessage
        {
            get { return _latestHeartRateMessage; }
        }

        public OxygenMessage LatestOxygenMessage
        {
            get { return _latestOxygenMessage; }
        }

        public GlucoseMessage LatestGlucoseMessage
        {
            get { return _latestGlucoseMessage; }
        }  

        public BostonMessage LatestBostonMessage
        {
            get { return _latestBostonMessage;  }
        }

        public ChicagoMessage LatestChicagoMessage
        {
            get { return _latestChicagoMessage; }
        }

        public NewYorkMessage LatestNewYorkMessage
        {
            get { return _latestNewYorkMessage; }
        }

        private async void HeartbeatTask()
        {
            while (true)
            {
                try
                {
                    SendMessageToHub(new HeartbeatMessage() { IsDefault = false });
                }
                catch { }

                await Task.Delay(1000 * 10).ConfigureAwait(false); // 10 seconds
            }
        }

        private void InitializeConfig()
        {
            try
            {
                _threadConfig = (ThreadConfigSection)ConfigurationManager.GetSection("ThreadConfig");
                //_hubConfig = (HubConfigSection)ConfigurationManager.GetSection("HubConfig");
            }
            catch (Exception ex)
            {
                Debug.Write("An error occurred during configuration" + ex.Message);
            }
        }

        private void InitializeContext()
        {
            _latestContextMessage = new ContextMessage
            {
                GeoFilter = GeoFilter.All, 
                TimeFilter = DateTime.Today
            };
        } 

        private void InitializeMessages()
        {
            _latestContextMessage = new ContextMessage
            {
                GeoFilter = GeoFilter.All, 
                TimeFilter = DateTime.Today
            };
            _latestSummaryMessage = new SummaryMessage() {  IsDefault = true };
            _latestTemperatureMessage = new TemperatureMessage() { IsDefault = true };
            _latestHeartRateMessage = new HeartRateMessage() { IsDefault = true };
            _latestOxygenMessage = new OxygenMessage() { IsDefault = true };
            _latestBostonMessage = new BostonMessage () { IsDefault = true };
            _latestChicagoMessage = new ChicagoMessage() { IsDefault = true };
            _latestNewYorkMessage = new NewYorkMessage() { IsDefault = true };
            _latestGlucoseMessage = new GlucoseMessage() { IsDefault = true }; 
            _sendToGroup = new Dictionary<string, Action>()
            {
                {GetMessageMetadata<ContextMessage>().Group, () => SendMessageToHub(LatestContextMessage)}, 
                {GetMessageMetadata<SummaryMessage>().Group, () => SendMessageToHub(LatestSummaryMessage)}, 
                {GetMessageMetadata<TemperatureMessage>().Group, () => SendMessageToHub(LatestTemperatureMessage)}, 
                {GetMessageMetadata<OxygenMessage>().Group, () => SendMessageToHub(LatestOxygenMessage)}, 
                {GetMessageMetadata<GlucoseMessage>().Group, () => SendMessageToHub(LatestGlucoseMessage)}, 
                {GetMessageMetadata<HeartRateMessage>().Group, () => SendMessageToHub(LatestHeartRateMessage)},
                {GetMessageMetadata<BostonMessage>().Group, () => SendMessageToHub(LatestBostonMessage)},
                {GetMessageMetadata<ChicagoMessage>().Group, () => SendMessageToHub(LatestChicagoMessage)},
                {GetMessageMetadata<NewYorkMessage>().Group, () => SendMessageToHub(LatestNewYorkMessage)}

            };
        }


        private void InitializeMonitors()
        {
            ConfigM configM = null;
            Manifest biometricsManifest = null;

            try
            {
                // lookup the biometrics api using the configuration service
                configM = new ConfigM { ApiUrl = ConfigurationManager.AppSettings["ConfigM"] };
                biometricsManifest = configM.GetByName("BiometricsAPI");

                _summaryMonitor = new SummaryMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _temperatureMonitor = new TemperatureMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _heartRateMonitor = new HeartRateMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _glucoseMonitor = new GlucoseMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _oxygenMonitor = new OxygenMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _bostonMonitor = new BostonMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _chicagoMonitor = new ChicagoMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
                _newYorkMonitor = new NewYorkMonitor(this) {api = biometricsManifest.lineitems["PublicAPI"]};
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }   

        public void SetContext(ContextMessage contextMessage)
        {
            if (contextMessage == null) return;

            var prev = _latestContextMessage;
            var newContextMessage = new ContextMessage
            {
                GeoFilter = contextMessage.GeoFilter, 
                TimeFilter = prev.TimeFilter
            };
            _latestContextMessage = newContextMessage;
            SendMessages();
        }

        private void SendMessages()
        {
            SendMessageToHub(_latestContextMessage);
            SendMessageToHub(_latestSummaryMessage);
            SendMessageToHub(_latestTemperatureMessage);
            SendMessageToHub(_latestHeartRateMessage);
        }

        public void SetGeoFilter(string geoFilter)
        {
            var latest = _latestContextMessage;

            var contextMessage = new ContextMessage
            {
                GeoFilter = (GeoFilter)Enum.Parse(typeof(GeoFilter),geoFilter), 
                TimeFilter = latest.TimeFilter
            };

            SetContext(contextMessage);
        }

        public void SetTimeFilter(DateTime timeFilter)
        {
            var latest = _latestContextMessage;


            var contextMessage = new ContextMessage
            {
                TimeFilter = timeFilter, 
                GeoFilter = latest.GeoFilter
            };

            SetContext(contextMessage);
        }  

        private T StampContextOnMessage<T>(T message) where T : BaseMessage
        {
            var context = _latestContextMessage;
            var cloned = message.Clone(); 
            cloned.TimeFilter = context.TimeFilter;
            cloned.GeoFilter = context.GeoFilter;

            return cloned;
        }
         
        private void SendMessageToHub<T>(T message) where T : BaseMessage
        {
            var attrib = GetMessageMetadata<T>();

            var clients = GlobalHost.ConnectionManager.GetHubContext<HealthHub>().Clients;

            var toSend = StampContextOnMessage(message);

            IClientProxy clientProxy = clients.All; // string.IsNullOrWhiteSpace(attrib.Group) ? clients.All : clients.Group(attrib.Group);
            clientProxy.Invoke(attrib.Name, toSend);
        }

        public void SendLatestToGroup(string group)
        {
            var action = _sendToGroup[group];

            action();
        }

        private static HealthMessageAttribute GetMessageMetadata<T>()
        {
            var attrib =
                Attribute.GetCustomAttribute(typeof(T), typeof(HealthMessageAttribute)) as
                    HealthMessageAttribute;
            if (attrib == null)
                throw new InvalidOperationException("Type " + typeof(T) + " is missing the HealthMessage attribute");

            return attrib;
        }

        public void UpdateSummaryMessage(SummaryMessage message)
        {
            if (message == null) return;

            _latestSummaryMessage = message;
            SendMessageToHub(_latestSummaryMessage);
        }

        public void UpdateTemperatureMessage(TemperatureMessage message)
        {
            if (message == null) return;

            _latestTemperatureMessage = message;
            SendMessageToHub(_latestTemperatureMessage);
        }

        public void UpdateHeartRateMessage(HeartRateMessage message)
        {
            if (message == null) return;

            _latestHeartRateMessage = message;
            SendMessageToHub(_latestHeartRateMessage);
        }

        public void UpdateOxygenMessage(OxygenMessage message)
        {
            if (message == null) return;

            _latestOxygenMessage = message;
            SendMessageToHub(_latestOxygenMessage);
        }

        public void UpdateGlucoseMessage(GlucoseMessage message)
        {
            if (message == null) return;

            _latestGlucoseMessage = message;
            SendMessageToHub(_latestGlucoseMessage);
        }

        public void UpdateBostonMessage(BostonMessage message)
        {
            if (message == null) return;

            _latestBostonMessage = message;
            SendMessageToHub(_latestBostonMessage);
        }

        public void UpdateChicagoMessage(ChicagoMessage message)
        {
            if (message == null) return;

            _latestChicagoMessage = message;
            SendMessageToHub(_latestChicagoMessage);
        }

        public void UpdateNewYorkMessage(NewYorkMessage message)
        {
            if (message == null) return;

            _latestNewYorkMessage = message;
            SendMessageToHub(_latestNewYorkMessage);
        } 

        public bool ContextChanged(BaseMessage msg)
        {
            // local copy of pointer
            var latestContext = _latestContextMessage;

            if (msg.GeoFilter != latestContext.GeoFilter ||
                msg.TimeFilter != latestContext.TimeFilter)
                return true;

            return false;
        }

        public List<Measurement> TempData { get; set; }
        public List<Measurement> GlucoseData { get; set; }
        public List<Measurement> OxygenData { get; set; }
        public List<Measurement> HeartRateData { get; set; }

        public int GetCountByCity(GeoFilter city)
        {
            var temp = TempData == null ? 0 : TempData.Where(d => d.Person.Location == city).Count();
            var glucose = GlucoseData == null ? 0 : GlucoseData.Where(d => d.Person.Location == city).Count();
            var oxygen = OxygenData == null ? 0 : OxygenData.Where(d => d.Person.Location == city).Count();
            var hr = HeartRateData == null ? 0 : HeartRateData.Where(d => d.Person.Location == city).Count(); 
            int count = 0;
            if (temp > count)
            {
                count = temp;
            }
            if (glucose > count)
            {
                count = glucose;
            }
            if (oxygen > count)
            {
                count = oxygen;
            }
            if (hr > count)
            {
                count = hr;
            }
            return count;
        }

        public int GetCountByState(Condition state)
        {
            var temp = TempData == null ? 0 : TempData.Where(d => d.State == state).Count();
            var glucose = GlucoseData == null ? 0 : GlucoseData.Where(d => d.State == state).Count();
            var oxygen = OxygenData == null ? 0 : OxygenData.Where(d => d.State == state).Count();
            var hr = HeartRateData == null ? 0 : HeartRateData.Where(d => d.State == state).Count();
            return temp + glucose + oxygen + hr;
        }

        public int GetCountByCityState(GeoFilter city, Condition state)
        {
            var temp = TempData == null ? 0 : TempData.Where(d => d.State == state && d.Person.Location == city).Count();
            var glucose = GlucoseData == null ? 0 : GlucoseData.Where(d => d.State == state && d.Person.Location == city).Count();
            var oxygen = OxygenData == null ? 0 : OxygenData.Where(d => d.State == state && d.Person.Location == city).Count();
            var hr = HeartRateData == null ? 0 : HeartRateData.Where(d => d.State == state && d.Person.Location == city).Count();
            return temp + glucose + oxygen + hr;
        }

        public List<Location> GetLocationsByCity(GeoFilter city)
        {
            var list = new List<Location>();

            var temp = TempData == null ? new List<Measurement>() : TempData.Where(d => d.Person.Location == city);
            var glucose = GlucoseData == null ? new List<Measurement>() : GlucoseData.Where(d => d.Person.Location == city);
            var oxygen = OxygenData == null ? new List<Measurement>() : OxygenData.Where(d => d.Person.Location == city);
            var hr = HeartRateData == null ? new List<Measurement>() : HeartRateData.Where(d => d.Person.Location == city);
            list.AddRange(ExtractLocations(temp));
            return list;
        }

        private List<Location> ExtractLocations(IEnumerable<Measurement> locations)
        {            
            var list = new List<Location>();
            foreach (var item in locations)
            {
                var location = new Location();
                location.latitude = item.Person.Latitude;
                location.longitude = item.Person.Longitude;
                list.Add(location);
            }
            return list;
        }

        public int GetPatientCount()
        {
            var count = 0;
            if (TempData != null && TempData.Count() > count)
            {
                count = TempData.Count();
            }
            if (GlucoseData != null && GlucoseData.Count() > count)
            {
                count = GlucoseData.Count();
            }
            if (OxygenData != null && OxygenData.Count() > count)
            {
                count = OxygenData.Count();
            }
            if (HeartRateData != null && HeartRateData.Count() > count)
            {
                count = HeartRateData.Count();
            }
            return count;
        }
    }
}