using System.Collections.Generic;

namespace KinderChat.ServiceBusShared.Entities
{
    public class GroupChatParticipant
    {
        public long UserId { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public List<string> Devices { get; set; } 
    }
}