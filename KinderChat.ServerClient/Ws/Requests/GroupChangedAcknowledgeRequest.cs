using System;
using KinderChat.ServerClient.Entities.Ws.Requests;

namespace KinderChat.ServerClient.Ws.Requests
{
    public class GroupChangedAcknowledgeRequest : BaseRequest
    {
        public Guid EventId { get; set; }
    }
}
