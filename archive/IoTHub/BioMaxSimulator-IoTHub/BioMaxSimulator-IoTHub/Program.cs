using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.Azure.Devices.Client;
using System.Threading;
using LooksFamiliar.Microservices.Common.Wire;
using LooksFamiliar.Microservices.Config.Models;
using LooksFamiliar.Microservices.Config.Public.SDK;
using LooksFamiliar.Microservices.Device.Admin.SDK;
using LooksFamiliar.Microservices.Device.Models;
using LooksFamiliar.Microservices.Profile.Models;
using LooksFamiliar.Microservices.Profile.Public.SDK;

namespace LooksFamiliar.Simulators.BioMax.IoTHub
{
    class Program
    {
        static readonly DeviceClient[] DeviceClients = new DeviceClient[300];
        static ConfigM _configM;
        static DeviceM _registryM;
        static ProfileM _profilesM;
        static Registrations _devices;
        static List<UserProfile> _profiles;

        static void Main(string[] args)
        {
            Console.WriteLine("************************************************************");
            Console.WriteLine("*  B I O M A X  S E N S O R  E V E N T  G E N E R A T O R  *");
            Console.WriteLine("*                                                          *");
            Console.WriteLine("*               I O T  H U B  E D I T I O N                *");
            Console.WriteLine("************************************************************");
            Console.WriteLine();
            Console.WriteLine("Press Enter to start the generator.");
            Console.WriteLine("Press Ctrl-C to stop the generator.");
            Console.WriteLine();
            Console.ReadLine();
            Console.Write("Working....");

            // initialize the ConfigM microservice client sdk
            _configM = new ConfigM {ApiUrl = ConfigurationManager.AppSettings["ConfigM"]};

            // lookup the manifests for the devie registry and user profile microservices
            var deviceManifest = _configM.GetByName("DeviceM");
            var profileManifest = _configM.GetByName("ProfileM");

            // initialize the DeviceM microservice client sdk
            _registryM = new DeviceM {ApiUrl = deviceManifest.lineitems[LineitemsKey.AdminAPI]};

            // initialize the ProfileM microservice client sdk
            _profilesM = new ProfileM {ApiUrl = profileManifest.lineitems[LineitemsKey.PublicAPI]};

            // get the device registry from the device microservice
            _devices = _registryM.GetAll();

            // get all the participants in the study
            _profiles = _profilesM.GetAllByType("Participant");

            // send simluated messages from the device collection
            SendDeviceToCloudMessagesAsync();

            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            var random = new Random();
            var spin = new ConsoleSpiner();

            while (true)
            {
                spin.Turn();

                try
                {
                    var deviceReading = new DeviceMessage();
                    var index = random.Next(0, _devices.list.Count - 1);

                    // randomly select a device from the registry
                    var device = _devices.list[index];

                    // lookup the participant associated with this device
                    var participant = _profiles.Find(p => p.id == device.participantid);

                    // create an IoT Hub client for this device if necessary
                    if (DeviceClients[index] == null)
                    {
                        // connect to the IoT Hub using unique device registration settings (deviceid, devicekey)
                        var deviceid = device.model + "-" + device.id;
                        DeviceClients[index] = DeviceClient.Create(
                            ConfigurationManager.AppSettings["IoTHubUri"], 
                            new DeviceAuthenticationWithRegistrySymmetricKey(deviceid, device.key));
                    }

                    // begin to create the simulated device message
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

                    // send the message to EventHub
                    DeviceClients[index].SendEventAsync(new Message(Encoding.ASCII.GetBytes(json))).Wait();
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(1000);
            }
        }
    }
}
