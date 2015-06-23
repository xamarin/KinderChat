using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using KinderChatServer.DAL;
using KinderChatServer.Models;

namespace KinderChatServer
{
    public class ApplicationUserManager : UserManager<UserDevice>
    {
        public ApplicationUserManager(IUserStore<UserDevice> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            /*
            Currently, we are using "ApplicationUserManager" as a way of storing user devices.
            The basic idea is that one "user" has many "devices". Since, by design, users don't have passwords, we need some way of "logging in"
            the user to make sure they are, you know, who they say they are. In order to do this, we take a normal ASP.NET Identity system 
            and generate the "Username" and "Password" on the server, then send it back to the client. Then when we need to "log in", the user device 
            uses those generated codes and in return gets an access token. This way they can create authed requests with their user instance.

            It's also how we assign public keys to individual devices. When the "device" gets created, we also attach the public key to it.
            So when another user wants to send a message to this users specific device, they now have a way to encrypt it.

            It's a massive hack that magically works. Does it scale? Probably not. But for the purposes of this demostration, it works.
            */
            var manager = new ApplicationUserManager(new UserStore<UserDevice>(context.Get<ApplicationDbContext>()));
            
            // Configure validation logic for user device ids
            manager.UserValidator = new UserValidator<UserDevice>(manager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
            // Configure validation logic for passwords (User device access tokens)
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<UserDevice>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
