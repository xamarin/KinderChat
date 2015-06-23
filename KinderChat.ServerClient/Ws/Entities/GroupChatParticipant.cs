using System.Collections.Generic;

namespace KinderChat.ServerClient.Ws.Entities
{

    public class GroupChatParticipant
    {
        public long UserId { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }
        
        public List<string> Devices { get; set; }
    }
}
