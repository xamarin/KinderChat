using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinderChat.ServerClient.Interfaces;

namespace KinderChat.ServerClient.Managers
{
    public class NotificationManager : INotificationManager
    {
        private readonly IWebManager _webManager;
        public NotificationManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public NotificationManager(string authenticationToken)
            : this(new WebManager(authenticationToken))
        {
        }

        public async Task<bool> SendPushNotification(string toDeviceId, string fromDeviceId, string fromUser)
        {
            var uri = new Uri(string.Format(EndPoints.PushMessage, toDeviceId, fromDeviceId, fromUser));
            var result = await _webManager.PostData(uri, null, null);
            if (!result.IsSuccess)
            {
                throw new Exception("Failed to send push message");
            }
            return result.IsSuccess;
        }
    }
}
