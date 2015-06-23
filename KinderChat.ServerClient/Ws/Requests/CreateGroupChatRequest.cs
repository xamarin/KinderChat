using System;
using System.Collections.Generic;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class CreateGroupChatRequest : BaseRequest
    {
        public List<long> Participants { get; set; }

        public string GroupName { get; set; }
    }
    

    public class CreateGroupChatResponse : BaseResponse
    {
        public Guid GroupId { get; set; }
    }
}
