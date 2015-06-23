using SQLite;

namespace KinderChat.Models
{
    public class DeviceInfo : EntityBase
    {
        [Indexed]
        public string DeviceId { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public string PublicKey { get; set; }
    }
}
