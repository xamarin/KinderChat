using System;

namespace KinderChat.ServiceBusShared.Entities
{
    public class Message : Event
    {
        public string SenderAccessToken { get; set; }

        public long SenderId { get; set; }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public bool IsSent { get; set; }
        
        public long ReceiverId { get; set; }
        
        public int MessageTypeId { get; set; }
        
        public Guid MessageToken { get; set; }
        
        public byte[] Thumbnail { get; set; }
        
        public string SenderDeviceId { get; set; }
        
        public byte[] EncryptionKey { get; set; }
        
        public Guid GroupId { get; set; }
    }
}
