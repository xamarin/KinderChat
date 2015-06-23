using System;
using System.Collections.Generic;
using KinderChat.ServerClient.Ws.Entities;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class GetGroupChatInfoRequest : BaseRequest
    {
        public Guid GroupId { get; set; }
    }

    public class GetGroupChatInfoResponse : BaseResponse
    {
        public GroupInfo GroupInfo { get; set; }
    }
}