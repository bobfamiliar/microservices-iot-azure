using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

/*
  NOTE: Service Bus Queue requires the EndPoint seeeting be present in your app.config or web.config file in order to work.
 * 
  <appSettings>
    <!-- Service Bus specific app setings for messaging connections -->
    <add key="Microsoft.ServiceBus.ConnectionString" value="Endpoint=sb://[your namespace].servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[your secret]" />
  </appSettings>
*/
namespace LooksFamiliar.Microservices.Common.Store
{
    public class Queue : IQueue
    {
        private static QueueClient _queueClient;

        public void Connect(string queueName)
        {
            var namespaceManager = NamespaceManager.Create();
            var queueDesc = namespaceManager.QueueExists(queueName) ? namespaceManager.GetQueue(queueName) : namespaceManager.CreateQueue(queueName);
            _queueClient = QueueClient.Create(queueDesc.Path);
        }

        public string Read()
        {
            string messageBody = null;
            BrokeredMessage brokeredMessage = null;
            brokeredMessage = _queueClient.Receive(TimeSpan.FromSeconds(5));
            if (brokeredMessage == null) return null;
            messageBody = brokeredMessage.GetBody<string>();
            brokeredMessage.Complete();
            return messageBody;
        }

        public void Write(string message)
        {
            var brokeredMessage = new BrokeredMessage(message) {MessageId = Guid.NewGuid().ToString()};
            _queueClient.Send(brokeredMessage);
        }
    }
}
