using System;
using System.Collections.Generic;

namespace KinderChat.ServiceBusShared.Entities
{
    public class GroupChat
    {
        public Guid GroupId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public long OwnerId { get; set; }

        public List<GroupChatParticipant> Participants { get; set; }
    }
}
