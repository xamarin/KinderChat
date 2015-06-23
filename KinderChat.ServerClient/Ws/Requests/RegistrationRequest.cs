using System.Collections.Generic;
using KinderChat.ServerClient.Entities.Ws.Events;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class AuthenticationRequest : BaseRequest
    {
        public string AccessToken { get; set; }
        
        public string DeviceId { get; set; }
        
        public long UserId { get; set; }
    }


    public class AuthenticationResponse : BaseResponse
    {
        public List<IncomingMessage> MissedMessages { get; set; }

        public string ServerInfo { get; set; }
    }
}
