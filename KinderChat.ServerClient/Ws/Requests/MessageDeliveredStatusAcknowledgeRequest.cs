using System;
using System.Collections.Generic;
using KinderChat.ServerClient.Entities.Ws.Events;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class MessageDeliveredStatusAcknowledgeRequest : BaseRequest
    {
        public List<Guid> Messages { get; set; }
    }
}