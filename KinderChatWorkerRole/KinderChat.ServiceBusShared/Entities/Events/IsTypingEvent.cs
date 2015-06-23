using System;

namespace KinderChat.ServiceBusShared.Entities
{
    public class IsTypingEvent : Event
    {
        public bool IsTyping { get; set; }
        
        public long SenderUserId { get; set; }
        
        public Guid GroupId { get; set; }
    }
}
