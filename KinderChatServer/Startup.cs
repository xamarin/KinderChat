using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Owin;
using KinderChat.ServiceBusShared;
using KinderChat.ServiceBusShared.Entities;
using KinderChatServer.Core;
using KinderChatServer.DAL;

[assembly: OwinStartup(typeof(KinderChatServer.Startup))]

namespace KinderChatServer
{
    public partial class Startup
    {
        private const string ConnectionString = "Endpoint=sb://kinderchat-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ggJ8eNLIkipevig9g7J2PzoW0h1iArizyA4XNMJtJJo=";

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = new PathString("/Images")
            });

            var messages = new ProcessedMessagesQueue(ConnectionString, OnMessageArrived);
        }

        private bool OnMessageArrived(Message messageDto)
        {
            var userTag = new string[2];

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var result = db.Users.FirstOrDefault(node => node.Id == messageDto.SenderId);
                    userTag[0] = "username:" + messageDto.ReceiverDeviceId;

                    var username = "Some rando";
                    var senderId = 0;
                    if (result != null)
                    {
                        username = result.NickName;
                        senderId = result.Id;
                        result.KinderPoints++;
                    }
                    userTag[1] = "from:" + senderId;
                    var push = new PushObject()
                    {
                        Message = username + " has sent you an encrypted message!",
                        FromId = senderId.ToString()
                    };
                    var notification = new Dictionary<string, string> { { "message", JsonConvert.SerializeObject(push) } };
                    //await Notifications.Instance.Hub.SendTemplateNotificationAsync(notification, userTag);
                    //await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                // TODO: Log error.
            }

            return true;
        }

        private class PushObject
        {
            public string Message { get; set; }

            public string FromId { get; set; }
        }
    }
}
