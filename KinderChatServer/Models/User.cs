using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace KinderChatServer.Models
{
    public class User
    {
        public int Id { get; set; }

        public virtual ICollection<UserDevice> Devices { get; set; }

        public virtual ICollection<AchievementsRecieved> Achievements { get; set; }

        public virtual ICollection<UserFlag> UserFlags { get; set; }

        public DateTime ConfirmTimestamp { get; set; }

        public DateTime Joined { get; set; }

        public string Email { get; set; }

        public string Sms { get; set; }

        public string NickName { get; set; }

        // TODO: When email is set up, don't send confirm key to client.
        //[JsonIgnore]
        public string ConfirmKey { get; set; }
        public int AvatarId { get; set; }

        public virtual Avatar Avatar { get; set; }

        public string KinderStatus { get; set; }

        public int KinderPoints { get; set; }

        public bool IsAgency { get; set; }

    }

    public class Avatar
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public AvatarType Type { get; set; }
    }

    public enum AvatarType
    {
        None,
        Animal,
        Food,
        Face,
        User
    }

    public class UserDevice : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserDevice> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }

        public int UserId { get; set; }

        public int AgencyId { get; set; }

        public string PublicKey { get; set; }

        [JsonIgnore]
        public int KinderPoints { get; set; }
    }
}