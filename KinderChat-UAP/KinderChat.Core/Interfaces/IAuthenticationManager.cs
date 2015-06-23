using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinderChat.Core.Entities;
using KinderChat.Core.Tools;

namespace KinderChat.Core.Interfaces
{
    public interface IAuthenticationManager
    {
        string Status { get; }

        Task<bool> GetTokenEmail(string email);

        Task<bool> GetTokenSms(string phone);

        Task<AuthTokens> CreateUserDeviceViaEmail(string email, string confirmKey, string publicKey);

        Task<bool> CreateUserDeviceViaPhoneNumber(string phone, string confirmKey, string publicKey);

        Task<AuthResponse> Authenticate(string userDeviceId, string accessToken);
    }
}
