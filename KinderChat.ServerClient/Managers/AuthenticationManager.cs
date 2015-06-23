using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KinderChat.ServerClient
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

        public async Task<AuthTokens> CreateUserDeviceViaEmail(string email, string confirmKey, string publicKey, string nickname)
        {
            var uri = new Uri(string.Format(EndPoints.CreateUserDeviceEmail, Uri.EscapeDataString(email), Uri.EscapeDataString(confirmKey), Uri.EscapeDataString(publicKey), Uri.EscapeDataString(nickname)));
            var result = await _webManager.PostData(uri, null, null);
            if (!result.IsSuccess)
            {
                throw new LoginFailedException("Failed to create user device: " + result.ResultJson);
            }
            return JsonConvert.DeserializeObject<AuthTokens>(result.ResultJson);
        }

        public Task<bool> CreateUserDeviceViaPhoneNumber(string phone, string confirmKey, string publicKey, string nickname)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> Authenticate(string userDeviceId, string accessToken)
        {
            var uri = new Uri(EndPoints.GetAccessToken);
            var body = new StringContent(string.Format("grant_type=password&username={0}&password={1}", userDeviceId, accessToken), Encoding.UTF8, "application/x-www-form-urlencoded");
            var result = await _webManager.PostData(uri, null, body);
            if (!result.IsSuccess)
            {
                throw new LoginFailedException("Failed to authenticate: " + result.ResultJson);
            }
            return JsonConvert.DeserializeObject<AuthResponse>(result.ResultJson);
        }
    }
}
