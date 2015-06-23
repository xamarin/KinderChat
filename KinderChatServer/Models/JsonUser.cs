using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KinderChatServer.Models
{
    public class JsonUser
    {
        public int Id { get; set; }

        public List<JsonUserDevice> Devices { get; set; }

        public List<JsonAchievement> Achievements { get; set; }

        public DateTime ConfirmTimestamp { get; set; }

        public string ConfirmKey { get; set; }

        public string Email { get; set; }

        public string Sms { get; set; }

        public string NickName { get; set; }

        public Avatar Avatar { get; set; }

        public string KinderStatus { get; set; }

        public int KinderPoints { get; set; }
    }

    public class JsonAchievement
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; }

        public string BadgeLocation { get; set; }
    }

    public class JsonUserDevice
    {
        public string DeviceId { get; set; }

        public string DeviceLoginId { get; set; }
        public string DeviceEmail { get; set; }

        public string DevicePhoneNumber { get; set; }

        public string PublicKey { get; set; }
    }
}