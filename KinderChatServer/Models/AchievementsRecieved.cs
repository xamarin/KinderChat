using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChatServer.Models
{
    public class AchievementsRecieved
    {
        public int Id { get; set; }

        public int AchievementId { get; set; }

        public virtual Achievement Achievement
        {
            get;
            set;
        }

        public int UserId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
