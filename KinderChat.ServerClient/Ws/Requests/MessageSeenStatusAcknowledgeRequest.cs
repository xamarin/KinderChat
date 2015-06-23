using System;
using System.Collections.Generic;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class MessageSeenStatusAcknowledgeRequest : BaseRequest
    {
        public List<Guid> Messages { get; set; }
    }
}