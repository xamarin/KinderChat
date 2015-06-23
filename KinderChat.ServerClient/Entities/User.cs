using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient.Entities
{
    public class User
    {
        public int Id { get; set; }

        public List<UserDevice> Devices { get; set; }

        public List<UserFlag> UserFlags { get; set; }

        public List<Achievement> Achievements { get; set; }

        public DateTime ConfirmTimestamp { get; set; }

        public string ConfirmKey { get; set; }

        public string Email { get; set; }

        public string Sms { get; set; }

        public string NickName { get; set; }

        public int AvatarId { get; set; }

        public Avatar Avatar { get; set; }

        public string KinderStatus { get; set; }

        public int KinderPoints { get; set; }
    }

    public class Avatar
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public AvatarType Type { get; set; }
    }

    public enum AvatarType
    {
        None,
        Animal,
        Food,
        Face,
        User
    }

    public class UserDevice
    {
        public string DeviceId { get; set; }
        public string DeviceLoginId { get; set; }
        public string DeviceEmail { get; set; }

        public string DevicePhoneNumber { get; set; }

        public string PublicKey { get; set; }
    }
}
