using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KinderChat.Core.Entities;
using KinderChat.Core.Exceptions;
using KinderChat.Core.Interfaces;
using KinderChat.Core.Tools;

namespace KinderChat.Core.Managers
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly IWebManager _webManager;

        public AuthenticationManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public AuthenticationManager()
            : this(new WebManager())
        {
        }

        public string Status { get; set; }
        public async Task<bool> GetTokenEmail(string email)
        {
            var uri = new Uri(string.Format(EndPoints.CreateUserEmail, email));
            var result = await _webManager.PostData(uri, null, null);
            if (!result.IsSuccess)
            {
                throw new LoginFailedException("Failed to send token email");
            }
            return result.IsSuccess;
        }

        public Task<bool> GetTokenSms(string phone)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthTokens> CreateUserDeviceViaEmail(string email, string confirmKey, string publicKey)
        {
            var uri = new Uri(string.Format(EndPoints.CreateUserDeviceEmail, email, confirmKey, publicKey));
            var result = await _webManager.PostData(uri, null, null);
            return JsonConvert.DeserializeObject<AuthTokens>(result.ResultJson);
        }

        public Task<bool> CreateUserDeviceViaPhoneNumber(string phone, string confirmKey, string publicKey)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> Authenticate(string userDeviceId, string accessToken)
        {
            var uri = new Uri(EndPoints.GetAccessToken);
            var body = new StringContent($"grant_type={"password"}&username={userDeviceId}&password={accessToken}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var result = await _webManager.PostData(uri, null, body);
            return JsonConvert.DeserializeObject<AuthResponse>(result.ResultJson);
        }
    }
}
