using System;
using Microsoft.Azure.NotificationHubs;
using NamespaceManager = Microsoft.ServiceBus.NamespaceManager;

namespace Looksfamiliar.Tools.SBUpdate
{
    class Program
    {
        public static string ConnectionString { get; private set; }
        public static string EventHubName { get; private set; }
        public static string QueueName { get; private set; }
        public static string NotificationHubName { get; private set; }

        static void Main(string[] args)
        {
            ConnectionString = string.Empty;
            EventHubName = string.Empty;
            QueueName = string.Empty;
            NotificationHubName = string.Empty;

            if (args.Length < 4)
            {
                Usage();
                return;
            }

            // parse command line arguments
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-connstr": 
                        i++;
                        ConnectionString = args[i];
                        break;
                    case "-eventhub": 
                        i++;
                        EventHubName = args[i];
                        break;
                    case "-queue": 
                        i++;
                        QueueName = args[i];
                        break;
                    case "-notificationhub":
                        i++;
                        NotificationHubName = args[i];
                        break;
                    case "?": // help
                        Usage();
                        break;
                    default: // default
                        Usage();
                        break;
                }
            }

            if (ConnectionString == string.Empty)
            {
                Console.WriteLine("ERROR: missing connection string.");
                Usage();
                return;
            }

            if ((EventHubName == string.Empty) && (QueueName == string.Empty) && (NotificationHubName == string.Empty))
            {
                Console.WriteLine("ERROR: missing event hub, queue name or notification hub.");
                Usage();
                return;
            }

            try
            {
                if (NotificationHubName != string.Empty)
                {
                    var nsclient = Microsoft.Azure.NotificationHubs.NamespaceManager.CreateFromConnectionString(ConnectionString);

                    if (!nsclient.NotificationHubExists(NotificationHubName))
                    {
                        var desc = new NotificationHubDescription(NotificationHubName);
                        nsclient.CreateNotificationHub(desc);
                    }
                }
                else
                {
                    var sbclient = NamespaceManager.CreateFromConnectionString(ConnectionString);

                    if (EventHubName != string.Empty)
                    {
                        if (!sbclient.EventHubExists(EventHubName))
                            sbclient.CreateEventHub(EventHubName);
                    }

                    if (QueueName != string.Empty)
                    {
                        if (!sbclient.QueueExists(QueueName))
                            sbclient.CreateQueue(QueueName);
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err.Message);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: SBUpdate -connstr [sb connection string] -eventhub [event hub name] -queue [queue name] -notificationhub [notification hub name]");
            Console.WriteLine("");
            Console.WriteLine("   -connstr           service bus connection string");
            Console.WriteLine("   -eventhub          name of the event hub to create");
            Console.WriteLine("   -queue             name of the queue to create");
            Console.WriteLine("   -notificationhub   name of the notification hub to create");
            Console.WriteLine("");
            Console.WriteLine("Note: each invocation of this utility will create one of either event hub, queue or notification hub.");
        }
    }
}
