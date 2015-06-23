using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient.Entities
{
    public class PopularNames
    {
        public string Nickname { get; set; }

        public int Count { get; set; }
    }

    public class PopularAvatars
    {
        public int AvatarId { get; set; }

        public int Count { get; set; }
    }

    public class RegDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Total { get; set; }
    }

    public class DeviceRegistration
    {
        public string Platform { get; set; }
        public string Handle { get; set; }
        public string[] Tags { get; set; }
    }
}
