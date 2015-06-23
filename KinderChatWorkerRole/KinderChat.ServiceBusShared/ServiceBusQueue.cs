using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace KinderChat.ServiceBusShared
{
    public class ServiceBusQueue
    {
        private static QueueClient queueClient;

        public event Action<BrokeredMessage> MessageReceived = delegate { }; 

        public void Initialize(string queueName, string connectionString)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            QueueDescription queue;
            if (!namespaceManager.QueueExists(queueName))
            {
                queue = namespaceManager.CreateQueue(queueName);
                queue.EnableBatchedOperations = false;
                queue.EnableExpress = true;
                queue.DefaultMessageTimeToLive = TimeSpan.FromMinutes(5);
                //queue.EnableLargeMessages = true;
                queue.MaxDeliveryCount = 100;
                //queue.RequiresSession = false;
                queue.SupportOrdering = true;
            }
            else
            {
                queue = namespaceManager.GetQueue(queueName);
            }

            queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName, ReceiveMode.PeekLock);
            queueClient.OnMessage(receivedMessage =>
            {
                try
                {
                    MessageReceived(receivedMessage);
                    Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                }
                catch (Exception exc)
                {
                    receivedMessage.Abandon();
                    Trace.TraceError(exc.ToString());
                }
            }, new OnMessageOptions { AutoComplete = false, MaxConcurrentCalls = 10000 });
        }

        public Task SendBatchAsync(IEnumerable<BrokeredMessage> messages)
        {
            return queueClient.SendBatchAsync(messages);
        }

        public Task SendAsync(BrokeredMessage message)
        {
            return queueClient.SendAsync(message);
        }

        public void Send(BrokeredMessage brokeredMessage)
        {
            queueClient.Send(brokeredMessage);
        }
    }
}
