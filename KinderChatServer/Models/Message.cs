using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChatServer.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int ToUserId { get; set; }

        public int FromUserId { get; set; }

        public string DeviceId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string EncryptedMessage { get; set; }
    }
}
