using System.Net.Mail;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using KinderChatServer.Models;
using Westwind.Web.Mvc;

namespace KinderChatServer.Core.EmailNotifications
{
    public class UserInvite : EmailNotification
    {
        public User User { get; }

        public string Email { get; }

        public HttpControllerContext Context { get;}

        public UserInvite(User user, string email, HttpControllerContext context)
        {
            User = user;
            Context = context;
            Email = email;
        }
        internal override void GenerateMessage()
        {
            MailAddress[] list = { new MailAddress(Email, "(Future) Kinder User")};
            Message.To = list;
            var name = User.NickName ?? User.Email;
            Message.Subject = name + " has invited you to Kinder Chat!";
            // TODO: Make a better email message.
            Message.Html = ViewRenderer.RenderView("~/views/templates/UserInvite.cshtml", User);
        }


    }
}
