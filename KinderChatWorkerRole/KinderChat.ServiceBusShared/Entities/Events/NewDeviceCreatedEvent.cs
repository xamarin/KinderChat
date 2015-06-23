using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServiceBusShared.Entities
{
    public class NewDeviceCreatedEvent : Event
    {
        public long UserId { get; set; }

        public string DeviceId { get; set; }
    }
}
