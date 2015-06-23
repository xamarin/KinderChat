using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KinderChat.ServerClient
{
    public class AuthTokens
    {
        [JsonProperty("AccessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("Username")]
        public string DeviceLoginId { get; set; }

        [JsonProperty("DeviceId")]
        public string DeviceId { get; set; }
    }
}
