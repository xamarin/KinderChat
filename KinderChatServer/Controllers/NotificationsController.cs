using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using KinderChatServer.Core;

namespace KinderChatServer.Controllers
{
    public class NotificationsController : ApiController
    {
        public async Task<HttpResponseMessage> Post(string toDeviceId, string fromDeviceId, string fromUser)
        {
            var userTag = new string[2];

            userTag[0] = "username:" + toDeviceId;
            userTag[1] = "from:" + fromUser;

            var notification = new Dictionary<string, string> { { "message", fromUser + " sent you a message!"} };
            await Notifications.Instance.Hub.SendTemplateNotificationAsync(notification, userTag);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
