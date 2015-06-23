using System;
using KinderChat.ServerClient.Entities.Ws.Requests;

namespace KinderChat.ServerClient.Ws.Requests
{
    public class LeaveGroupRequest : BaseRequest
    {
        public Guid GroupId { get; set; }
    }

    public class LeaveGroupResponse : BaseResponse
    {
    }
}
