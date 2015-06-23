using System.Net.Mail;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using KinderChatServer.Models;
using Westwind.Web.Mvc;

namespace KinderChatServer.Core.EmailNotifications
{
    public class UserAccountConfirmation : EmailNotification
    {
        public User User { get; }

        public HttpControllerContext Context { get;}

        public UserAccountConfirmation(User user, HttpControllerContext context)
        {
            User = user;
            Context = context;
        }
        internal override void GenerateMessage()
        {
            MailAddress[] list = { new MailAddress(User.Email, "Kinder User") };
            Message.To = list;
            Message.Subject = "Confirm your Kinder Chat Account!";
            // TODO: Make a better email message.
            Message.Html = ViewRenderer.RenderView("~/views/templates/NewUser.cshtml", User);
        }


    }
}
