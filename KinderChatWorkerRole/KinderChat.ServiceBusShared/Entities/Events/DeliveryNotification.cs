using System;

namespace KinderChat.ServiceBusShared.Entities
{
    public class DeliveryNotification : Event
    {
        public Guid MessageToken { get; set; }
    }
}