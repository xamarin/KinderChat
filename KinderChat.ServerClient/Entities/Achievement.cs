using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient.Entities
{
    public class Achievement
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; }

        public string BadgeLocation { get; set; }
    }
}
