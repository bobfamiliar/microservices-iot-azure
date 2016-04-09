using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

class Program
{
    static void Main(string[] args)
    {
        const string iotHubD2CEndpoint = "messages/events";
        var connectionString = ConfigurationManager.AppSettings["iothubconnstr"];

        var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2CEndpoint);

        var d2CPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

        foreach (var receiver in d2CPartitions.Select(partition => eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now)))
        {
            ReceiveMessagesFromDeviceAsync(receiver);
        }
        Console.ReadLine();
    }

    async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
    {
        while (true)
        {
            var eventData = await receiver.ReceiveAsync();
            if (eventData == null) continue;

            var data = Encoding.UTF8.GetString(eventData.GetBytes());
            Console.WriteLine("Message received: '{0}'", data);
        }
    }
}

