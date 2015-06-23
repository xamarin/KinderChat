using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;

namespace KinderChatServer.Core
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://kinder-chat.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=fJTzehXef3r9GTuhaouH47Qm92naTGkbNMZl2I1GtGE=",
                "kinder-chat-messages");
        }
    }
}
