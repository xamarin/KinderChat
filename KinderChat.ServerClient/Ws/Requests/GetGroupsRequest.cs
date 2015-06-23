using System;
using System.Collections.Generic;
using KinderChat.ServerClient.Ws.Entities;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class GetGroupsRequest : BaseRequest
    {
    }

    public class GetGroupsResponse : BaseResponse
    {
        public List<GroupInfo> Groups { get; set; } 
    }
}