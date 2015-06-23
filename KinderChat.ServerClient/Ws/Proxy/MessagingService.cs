using System;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities.Ws.Events;
using KinderChat.ServerClient.Entities.Ws.Requests;

namespace KinderChat.ServerClient.Ws.Proxy
{
    /// <summary>
    /// Proxy for MessagesController
    /// TODO: generate via T4 script
    /// </summary>
    public class MessagingService
    {
        private readonly ConnectionManager _connectionManager;

        public MessagingService(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _connectionManager.EventPushed += OnEventPushed;
        }

        public event Action<DeliveryNotification> DeliveryNotification = delegate { };
        public event Action<IncomingMessage> IncomingMessage = delegate { };
        public event Action<IsTypingNotification> IsTypingNotification = delegate { };
        public event Action<SeenNotification> SeenNotification = delegate { }; 

        private void OnEventPushed(Events.PushedEvent pushedEvent)
        {
            if (pushedEvent is DeliveryNotification)
                DeliveryNotification((DeliveryNotification)pushedEvent);
            else if (pushedEvent is IncomingMessage)
                IncomingMessage((IncomingMessage)pushedEvent);
            else if (pushedEvent is IsTypingNotification)
                IsTypingNotification((IsTypingNotification)pushedEvent);
            else if (pushedEvent is SeenNotification)
                SeenNotification((SeenNotification)pushedEvent);
        }

        public Task<SendMessageResponse> SendMessage(SendMessageRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<SendMessageResponse>(request);
        }

        public Task SendIsTyping(SendIsTypingRequest request)
        {
            return _connectionManager.SendRequest(request);
        }

        public Task MessageSeenStatusAcknowledge(MessageSeenStatusAcknowledgeRequest request)
        {
            return _connectionManager.SendRequest(request);
        }

        public Task MessageReceivedStatusAcknowledge(MessageReceivedStatusAcknowledgeRequest request)
        {
            return _connectionManager.SendRequest(request);
        }

        public Task MessageDeliveredStatusAcknowledge(MessageDeliveredStatusAcknowledgeRequest request)
        {
            return _connectionManager.SendRequest(request);
        }

        public Task<MarkMessageAsSeenResponse> MarkMessageAsSeen(MarkMessageAsSeenRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<MarkMessageAsSeenResponse>(request);
        }
    }
}