using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LooksFamiliar.Microservices.Biometrics.Models;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Public.SDK;
using LooksFamiliar.Microservices.Profile.Public.SDK;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.NotificationHubs;
using Microsoft.WindowsAzure.ServiceRuntime;
using ServiceBusConnectionStringBuilder = Microsoft.ServiceBus.ServiceBusConnectionStringBuilder;
using TransportType = Microsoft.ServiceBus.Messaging.TransportType;

namespace Alarms
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);


        public override void Run()
        {
            Trace.TraceInformation("Alarms is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            Trace.TraceInformation("Alarms has been started");

            var eventHubName = RoleEnvironment.GetConfigurationSettingValue("EventHubName");
            var serviceBusConnectionString = RoleEnvironment.GetConfigurationSettingValue("Azure.ServiceBus.ConnectionString");
            var storageConnectionString = RoleEnvironment.GetConfigurationSettingValue("Azure.Storage.ConnectionString");

            var builder = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
            builder.TransportType = TransportType.Amqp;

            var eventHubReceiveClient = EventHubClient.CreateFromConnectionString(builder.ToString(), eventHubName);

            var eventHubConsumerGroup = eventHubReceiveClient.GetDefaultConsumerGroup();

            var eventProcessorHost = new EventProcessorHost( "AlarmsWorker",
                                                             eventHubName,
                                                             eventHubConsumerGroup.GroupName,
                                                             builder.ToString(),
                                                             storageConnectionString);

            eventProcessorHost.RegisterEventProcessorAsync<MessageProcessor>();

            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Alarms is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Alarms has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }

    public class MessageProcessor : IEventProcessor
    {
        private ConfigM _config;
        private ProfileM _profile;
        private string _biometricsApi;
        private NotificationHubClient _hub;

        public Task OpenAsync(PartitionContext context)
        {
            // get the manifests for the profile and biometrics services
            _config = new ConfigM { ApiUrl = RoleEnvironment.GetConfigurationSettingValue("ConfigM") };

            var profileManifest = _config.GetByName("ProfileM");
            _profile = new ProfileM { ApiUrl = profileManifest.lineitems["PublicAPI"] };

            var biometricsManifest = _config.GetByName("BiometricsAPI");
            _biometricsApi = biometricsManifest.lineitems["PublicAPI"] + "/alarm";

            // connect to notification hub
            _hub = NotificationHubClient.CreateClientFromConnectionString(
                RoleEnvironment.GetConfigurationSettingValue("NotificationHubConnectionString"),
                RoleEnvironment.GetConfigurationSettingValue("NotificationHubName"));

            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                // get the alarm from event hub
                var stream = eventData.GetBodyStream();
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                var json = bytes.Aggregate(string.Empty, (current, t) => current + ((char)t).ToString());

                BiometricReading alarm = null;

                try
                {
                    alarm = ModelManager.JsonToModel<BiometricReading>(json);
                }
                catch (Exception)
                {
                    continue;
                }

                // log the alarm to biometrics database using the API
                //Rest.Post(new Uri(_biometricsApi), json);

                // lookup the user that rasied the alarm
                var user = _profile.GetById(alarm.participantid);

                //format the toast message
                var biometric = string.Empty;
                switch (alarm.type)
                {
                    case BiometricType.Glucose:
                        biometric = "Glucose";
                        break;
                    case BiometricType.Heartrate:
                        biometric = "Heartrate";
                        break;
                    case BiometricType.Temperature:
                        biometric = "Tempurature";
                        break;
                    case BiometricType.Bloodoxygen:
                        biometric = "Blood Oxygen";
                        break;
                    case BiometricType.NotSet:
                        break;
                    default:
                        biometric = "Not Set";
                        break;
                }

                var toast = "<toast><visual><binding template = 'ToastText04'> " +
                            $"<text id = '1'>{"BioMax Alert"}</text>" +
                            $"<text id = '2'>{"The " + biometric + " reading for " + user.firstname + " " + user.lastname + " is out of range."}</text>" +
                            $"<text id = '3' >{"Contact: " + user.social.phone}</text>" + "</binding ></visual></toast>";

                _hub.SendWindowsNativeNotificationAsync(toast).Wait();
            }

            await context.CheckpointAsync();
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }
    }
}
