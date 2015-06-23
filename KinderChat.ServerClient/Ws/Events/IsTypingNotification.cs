using System;
using KinderChat.ServerClient.Ws.Events;

namespace KinderChat.ServerClient.Entities.Ws.Events
{
    public class IsTypingNotification : PushedEvent
    {
        public long SenderUserId { get; set; }

        public bool IsTyping { get; set; }
        
        public Guid GroupId { get; set; }
    }
}
