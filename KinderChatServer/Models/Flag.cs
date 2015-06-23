using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChatServer.Models
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

        public virtual Flag Flag { get; set; }

        public virtual User AccusedUser { get; set; }

        public virtual User AccuserUser { get; set; }
    }

    public enum FlagAlertLevel
    {
        Low,
        Medium,
        High
    }
}
