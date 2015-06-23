using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KinderChat.Core.Entities
{
    public class AuthTokens
    {
        [JsonProperty("AccessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("Username")]
        public string UserDeviceId { get; set; }
    }
}
