using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChatServer.Models
{
    public class Friend
    {
        public int Id { get; set; }

        public int FriendUserId { get; set; }

        public virtual User FriendUser { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
