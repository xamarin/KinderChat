using System;
using System.Collections.Generic;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class KickParticipantsRequest : BaseRequest
    {
        public Guid GroupId { get; set; }

        public List<long> ParticipantIds { get; set; }
    }

    public class KickParticipantsResponse : BaseResponse
    {
    }
}