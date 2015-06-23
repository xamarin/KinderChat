using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities;

namespace KinderChat.ServerClient.Interfaces
{
    public interface IUserManager
    {
        Task<User> GetUser(int id);

        Task<User> GetUser(string email);

        Task<UserFlag> FlagUser(int id, int flagid);

        Task<List<Flag>> GetFlags();

        Task<bool> SendUserInvite(string email);

        Task<bool> SendLinkEmailToCurrentUser(string email);

        Task<bool> SendLinkPhoneToCurrentUser(string phone);

        Task<bool> LinkPhoneNumberToCurrentUser(string phone, string confirmKey);

        Task<bool> LinkEmailToCurrentUser(string email, string confirmKey);

        Task<bool> ChangeNickname(string nickname);

        Task<Avatar> SetAvatarFromList(int id);

        Task<Avatar> AddCustomAvatar(Stream image);

        Task<bool> UpdateUserProfile(User user);

        Task<bool> VerifyUserExistsViaEmail(string email);

        Task<bool> VerifyUserExistsViaPhone(string phone);
    }
}
