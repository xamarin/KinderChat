using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid;

namespace KinderChatServer.Core
{
    public abstract class EmailNotification
    {
        protected SendGridMessage Message { get; }

        protected EmailNotification()
        {
            Message = new SendGridMessage();
            Message.AddTo("Admin <admin@example.com>");
        }

        internal abstract void GenerateMessage();

        public void Send()
        {
            GenerateMessage();

            try
            {
                var credentials = new NetworkCredential("azure_4b721d9081d6ff95ef3fb5333bd60dd7@azure.com", "KHCLTE94rvwQ0dS");
                Message.From = new MailAddress("admin@kinder-chat.com", "Admin");
                var transportWeb = new Web(credentials);
                transportWeb.Deliver(Message);
            }
            catch (Exception ex)
            {
                //Logger.Error("Error sending email", new { Exception = ex });
            }
        }
    }
}
