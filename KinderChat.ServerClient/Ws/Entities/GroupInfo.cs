using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient.Ws.Entities
{
    public class GroupInfo
    {
        public Guid GroupId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public long OwnerId { get; set; }

        public List<GroupChatParticipant> Participants { get; set; }
    }
}
