using System;
using KinderChat.ServerClient.Ws.Events;

namespace KinderChat.ServerClient.Entities.Ws.Events
{
    public class DeliveryNotification : PushedEvent
    {
        public Guid EventId { get; set; }

        public Guid MessageToken { get; set; }
        
        public DateTime DeliveredAt { get; set; }
    }
}
