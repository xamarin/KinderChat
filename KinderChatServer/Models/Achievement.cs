using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChatServer.Models
{
    public class Achievement
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string BadgeLocation { get; set; }
    }
}
