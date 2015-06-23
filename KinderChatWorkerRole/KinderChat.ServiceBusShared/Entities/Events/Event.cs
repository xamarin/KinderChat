using System;

namespace KinderChat.ServiceBusShared.Entities
{
    public abstract class Event
    {
        public Guid EventId { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public string ReceiverDeviceId { get; set; }
    }
}