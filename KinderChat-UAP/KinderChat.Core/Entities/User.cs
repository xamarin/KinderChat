using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public List<UserDevice> Devices { get; set; }

        public DateTime ConfirmTimestamp { get; set; }

        public string ConfirmKey { get; set; }

        public string Email { get; set; }

        public string Sms { get; set; }

        public string Avatar { get; set; }

        public string KinderStatus { get; set; }

        public int KinderPoints { get; set; }
    }

    public class UserDevice
    {
        public string DeviceEmail { get; set; }

        public string DevicePhoneNumber { get; set; }

        public string PublicKey { get; set; }
    }
}
