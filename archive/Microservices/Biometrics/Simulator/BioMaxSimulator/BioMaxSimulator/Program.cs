using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Public.SDK;
using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Device.Admin.SDK;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Profile.Public.SDK;
using Microsoft.ServiceBus.Messaging;

namespace LooksFamiliar.Simulators.BioMax
{
    class Program
    {
        static ConfigM _configM;
        static DeviceM _registryM;
        static ProfileM _profilesM;
        static Registrations _devices;
        static List<UserProfile> _profiles;

        static void Main(string[] args)
        {
            Console.WriteLine("************************************************************");
            Console.WriteLine("*  B I O M A X  S E N S O R  E V E N T  G E N E R A T O R  *");
            Console.WriteLine("************************************************************");
            Console.WriteLine();
            Console.WriteLine("Press Enter to start the generator.");
            Console.WriteLine("Press Ctrl-C to stop the generator.");
            Console.WriteLine();
            Console.ReadLine();
            Console.Write("Working....");

            _configM = new ConfigM();
            _registryM = new DeviceM();
            _profilesM = new ProfileM();

            // the endpoiont for ConfigM is defined in the app config
            _configM.ApiUrl = ConfigurationManager.AppSettings["ConfigM"];

            var deviceManifest = _configM.GetByName("DeviceM");
            var profileManifest = _configM.GetByName("ProfileM");

            _registryM.ApiUrl = deviceManifest.lineitems[LineitemsKey.AdminAPI];
            _profilesM.ApiUrl = profileManifest.lineitems[LineitemsKey.PublicAPI];

            // get the device registry from the device microservice
            _devices = _registryM.GetAll();

            // get all the participants in the study
            _profiles = _profilesM.GetAllByType("Participant");

            var random = new Random();
            var spin = new ConsoleSpiner();

            // connect to Event Hub
            var servicebus = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var eventhub = ConfigurationManager.AppSettings["EventHub"];
            var eventHubClient = EventHubClient.CreateFromConnectionString(servicebus, eventhub);

            while (true)
            {
                spin.Turn();

                try
                {
                    var deviceReading = new DeviceMessage();
                    var index = random.Next(0, _devices.list.Count - 1);
                    
                    // randomly select a device from the registry
                    var device = _devices.list[index];

                    // lookup the participant
                    var participant = _profiles.Find(p => p.id == device.participantid);

                    // beging to create the simulated device message
                    deviceReading.deviceid = device.id;
                    deviceReading.participantid = participant.id;
                    deviceReading.location.latitude = participant.location.latitude;
                    deviceReading.location.longitude = participant.location.longitude;

                    // generate simulated sensor reaings
                    var glucose = new SensorReading
                    {
                        type = SensorType.Glucose,
                        value = random.Next(70, 210)
                    };

                    var heartrate = new SensorReading
                    {
                        type = SensorType.Heartrate,
                        value = random.Next(60, 180)
                    };

                    var temperature = new SensorReading
                    {
                        type = SensorType.Temperature,
                        value = random.Next(98, 105) + (.1 * random.Next(0, 9))
                    };

                    var bloodoxygen = new SensorReading
                    {
                        type = SensorType.Bloodoxygen,
                        value = random.Next(80, 100)
                    };

                    deviceReading.sensors.Add(glucose);
                    deviceReading.sensors.Add(heartrate);
                    deviceReading.sensors.Add(temperature);
                    deviceReading.sensors.Add(bloodoxygen);

                    deviceReading.reading = DateTime.Now;

                    // serialize the message to JSON
                    var json = ModelManager.ModelToJson<DeviceMessage>(deviceReading);
                    
                    // use these lines to gen JSON files for SA test input
                    //var filename = AppDomain.CurrentDomain.BaseDirectory + @"\data\device-" + DateTime.Now.Ticks + ".json";
                    //System.IO.File.WriteAllText(filename, json);

                    // send the message to EventHub
                    eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(json)));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(200);
            }
        }
     }
}
