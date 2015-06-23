using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using KinderChatServer.Models;

namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KinderChatServer.DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(KinderChatServer.DAL.ApplicationDbContext context)
        {
            if (!(context.Users.Any(u => u.Email == "nsa@nsa.gov")))
            {
                var user = new User()
                {
                    NickName = "NSA",
                    Email = "nsa@nsa.gov",
                    ConfirmTimestamp = DateTime.UtcNow,
                    Sms = "333-444-3434",
                    IsAgency = true,
                    AvatarId = 1
                };
                context.Users.AddOrUpdate(user);
                context.SaveChanges();

                var userStore = new UserStore<UserDevice>(context);
                var userManager = new UserManager<UserDevice>(userStore);
                var userToInsert = new UserDevice { UserName = GetUniqueKey(64), Email = "nsa@nsa.gov", UserId = user.Id, PublicKey = GetUniqueKey(64) };
                userManager.Create(userToInsert, GetUniqueKey(64));
            }

        }

        private static string GetUniqueKey(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
