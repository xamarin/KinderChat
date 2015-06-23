using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinderChat.Core.Tools
{
    public class EndPoints
    {
        public const int DefaultTimeoutInMilliseconds = 60000;

        public const string BaseUrl = "https://kinder-chat.azurewebsites.net";

        public const string CreateUserEmail = BaseUrl + "/api/Users/CreateUser?email={0}";

        public const string CreateUserDeviceEmail =
            BaseUrl + "/api/users/CreateUserDevice?email={0}&confirmKey={1}&publicKey={2}";

        public const string GetAccessToken = BaseUrl + "/Token";
    }
}
