using System;
using KinderChat.ServerClient.Entities.Ws.Events;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class ChangeGroupRequest : BaseRequest
    {
        public Guid GroupId { get; set; }

        public string NewGroupName { get; set; }

        public string NewGroupAvatar { get; set; }
    }

    public class ChangeGroupResponse : BaseResponse
    {
    }
}