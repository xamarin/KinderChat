using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient.Entities
{
    public class Flag
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public FlagAlertLevel AlertLevel { get; set; }
    }

    public class UserFlag
    {
        public int Id { get; set; }

        public int FlagId { get; set; }

        public bool Resolved { get; set; }

        public int AccusedUserId { get; set; }

        public int AccuserUserId { get; set; }

        public Flag Flag { get; set; }

        public User AccusedUser { get; set; }

        public User AccuserUser { get; set; }
    }

    public enum FlagAlertLevel
    {
        Low,
        Medium,
        High
    }
}
