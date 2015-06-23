using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace KinderChat.ServiceBusShared
{
    public class ServiceBusTopic
    {
        private TopicClient _topicClient;

        public event Action<BrokeredMessage> MessageReceived = delegate { };

        public void Initialize(string topicName, string subscriptionName, string filterExpression, string connectionString)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.TopicExists(topicName))
            {
                var topicDesc = new TopicDescription(topicName);
                topicDesc.DefaultMessageTimeToLive = TimeSpan.FromMinutes(10);
                topicDesc.EnableBatchedOperations = false;
                topicDesc.EnableExpress = false;
                topicDesc.SupportOrdering = true;
                var topic = namespaceManager.CreateTopic(topicDesc);
            }

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
            {
                var subDesc = new SubscriptionDescription(topicName, subscriptionName);
                subDesc.EnableBatchedOperations = false;
                namespaceManager.CreateSubscription(subDesc, new SqlFilter(filterExpression /*"StoreName = 'Store1'"*/));
            }

            var client = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);
            client.PrefetchCount = 0;
            client.MessagingFactory.PrefetchCount = 0;
            //client.MessagingFactory.RetryPolicy = RetryExponential.Default;

            _topicClient = TopicClient.CreateFromConnectionString(connectionString, topicName);


            client.OnMessage(receivedMessage =>
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
            return _topicClient.SendBatchAsync(messages);
        }

        public Task SendAsync(BrokeredMessage message)
        {
            return _topicClient.SendAsync(message);
        }

        public void Send(BrokeredMessage brokeredMessage)
        {
            _topicClient.Send(brokeredMessage);
        }
    }
}
