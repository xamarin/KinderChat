using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KinderChat.ServerClient.Entities;
using KinderChat.ServerClient.Interfaces;

namespace KinderChat.ServerClient.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IWebManager _webManager;
        public UserManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public UserManager(string authenticationToken)
            : this(new WebManager(authenticationToken))
        {
        }

        public async Task<User> GetUser(int id)
        {
            var uri = new Uri(string.Format(EndPoints.GetUserViaId, id));
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<User>(result.ResultJson);
        }

        public async Task<User> GetUser(string email)
        {
            var uri = new Uri(string.Format(EndPoints.GetUserViaEmail, email));
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<User>(result.ResultJson);
        }

        public async Task<UserFlag> FlagUser(int id, int flagid)
        {
            var uri = new Uri(string.Format(EndPoints.FlagUser, id, flagid));
            var result = await _webManager.PostData(uri, null, null);
            if (!result.IsSuccess)
            {
                throw new Exception("Failed to set user flag: " + result.ResultJson);
            }
            return JsonConvert.DeserializeObject<UserFlag>(result.ResultJson);
        }

        public async Task<List<Flag>> GetFlags()
        {
            var uri = new Uri(EndPoints.GetFlags);
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<List<Flag>>(result.ResultJson);
        }

        public async Task<bool> SendUserInvite(string email)
        {
            var uri = new Uri(string.Format(EndPoints.SendUserInvite, email));
            var result = await _webManager.GetData(uri);
            return result.IsSuccess;
        }

        public Task<bool> SendLinkEmailToCurrentUser(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendLinkPhoneToCurrentUser(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LinkPhoneNumberToCurrentUser(string phone, string confirmKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LinkEmailToCurrentUser(string email, string confirmKey)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ChangeNickname(string nickname)
        {
            var uri = new Uri(string.Format(EndPoints.ChangeNickname, nickname));
            var result = await _webManager.PostData(uri, null, null);
            return result.IsSuccess;
        }

        public async Task<Avatar> SetAvatarFromList(int id)
        {
            var uri = new Uri(string.Format(EndPoints.AddAvatar, id));
            var result = await _webManager.PostData(uri, null, null);
            return JsonConvert.DeserializeObject<Avatar>(result.ResultJson);
        }

        public async Task<Avatar> AddCustomAvatar(Stream stream)
        {
            var uri = new Uri(EndPoints.ChangeAvatar);
            StreamContent t;
            try
            {
                t = new StreamContent(stream);
                t.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                t.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                t.Headers.Add("Content-Description", "image-data-0");
                t.Headers.Add("Content-Transfer-Encoding", "binary");
                t.Headers.ContentLength = stream.Length;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create image", ex);
            }

            try
            {
                var form = new MultipartContent("mixed", "acebdf13572468") { t };
                var result = await _webManager.PostData(uri, form, null);
                return JsonConvert.DeserializeObject<Avatar>(result.ResultJson);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send image", ex);
            }
        }

        public Task<bool> UpdateUserProfile(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> VerifyUserExistsViaEmail(string email)
        {
            var uri = new Uri(string.Format(EndPoints.VerifyUserExistsViaEmail, email));
            var result = await _webManager.GetData(uri);
            return result.IsSuccess;
        }

        public async Task<bool> VerifyUserExistsViaPhone(string phone)
        {
            var uri = new Uri(string.Format(EndPoints.VerifyUserExistsViaPhone, phone));
            var result = await _webManager.GetData(uri);
            return result.IsSuccess;
        }
    }
}
