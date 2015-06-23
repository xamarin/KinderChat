using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.ServerClient.Interfaces
{
    public interface INotificationManager
    {
        Task<bool> SendPushNotification(string toDeviceId, string fromDeviceId, string fromUser);
    }
}
