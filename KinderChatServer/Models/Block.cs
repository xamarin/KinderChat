using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChatServer.Models
{
    public class Block
    {
        public int Id { get; set; }

        public int BlockUserId { get; set; }

        public virtual User BlockUser { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
