using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Models.Entities;
using Models.Enums;
using Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BiometricsDashboard.Hubs
{
    [HubName("HealthHub")]
    public class HealthHub : Hub
    {
        private HealthHubManager _hubManager = HealthHubManager.Instance;

        public void setContext(ContextMessage contextMessage)
        {
            _hubManager.SetContext(contextMessage);
        }

        public void setFilter(string filter)
        {
            if (_hubManager == null) return;
            if (string.IsNullOrWhiteSpace(filter)) return;
            string[] filterParts = filter.Split('|');
            if (filterParts == null) return;
            if (filterParts.Length != 3) return;

            this.setContext(new ContextMessage
            {
                GeoFilter = (GeoFilter) Enum.Parse(typeof(GeoFilter), filterParts[0]), 
                TimeFilter = Convert.ToDateTime(filterParts[2])
            });
        }

        public void setGeoFilter(string geoFilter)
        {
            if (_hubManager == null) return;
            _hubManager.SetGeoFilter(geoFilter);
        }
         
        public void setTimeFilter(DateTime timeFilter)
        {
            if (_hubManager == null) return;
            _hubManager.SetTimeFilter(timeFilter);
        } 

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            Debug.Write(string.Format("Client {0} connected", this.Context.ConnectionId));

            return base.OnConnected();
        }

        public async Task JoinGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new HubException("GroupName must not be empty");

            await Groups.Add(Context.ConnectionId, groupName);

            Debug.Write(string.Format("Client {0} joined group {1}", this.Context.ConnectionId, groupName));

            //send latest message immediately
            _hubManager.SendLatestToGroup(groupName);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name. 
            Debug.Write(string.Format("Client {0} disconnected", this.Context.ConnectionId));
            
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            Debug.Write(string.Format("Client {0} reconnected", this.Context.ConnectionId));
            return base.OnReconnected();
        }

        #region GetMessages

        public SummaryMessage getLatestSummaryMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestSummaryMessage;
        }

        public TemperatureMessage getLatestTemperatureMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestTemperatureMessage;
        }

        public HeartRateMessage getLatestHeartRateMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestHeartRateMessage;
        }

        public GlucoseMessage getLatestGlucoseMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestGlucoseMessage;
        }

        public OxygenMessage getLatestOxygenMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestOxygenMessage;
        }

        public BostonMessage getLatestBostonMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestBostonMessage;
        }

        public ChicagoMessage getLatestChicagoMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestChicagoMessage;
        }

        public NewYorkMessage getLatestNewYorkMessage()
        {
            return _hubManager == null ? null : _hubManager.LatestNewYorkMessage;
        }
        #endregion

    }
}