using System;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Events;
using KinderChat.ServerClient.Ws.Requests;

namespace KinderChat.ServerClient.Ws.Proxy
{
    /// <summary>
    /// Proxy for GroupChatsController
    /// TODO: generate via T4 script
    /// </summary>
    public class GroupChatsService 
    {
        private readonly ConnectionManager _connectionManager;

        public GroupChatsService(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _connectionManager.EventPushed += OnEventPushed;
        }

        public event Action<GroupChangedNotification> GroupChanged = delegate { };

        private void OnEventPushed(PushedEvent pushedEvent)
        {
            if (pushedEvent is GroupChangedNotification)
                GroupChanged((GroupChangedNotification)pushedEvent);
        }

        public Task<CreateGroupChatResponse> CreateGroupChat(CreateGroupChatRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<CreateGroupChatResponse>(request);
        }

        public Task<ChangeGroupResponse> ChangeGroup(ChangeGroupRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<ChangeGroupResponse>(request);
        }

        public Task<LeaveGroupResponse> LeaveGroup(LeaveGroupRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<LeaveGroupResponse>(request);
        }

        public Task<GetGroupChatInfoResponse> GetGroupChatInfo(GetGroupChatInfoRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<GetGroupChatInfoResponse>(request);
        }

        public Task<GetGroupsResponse> GetGroups(GetGroupsRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<GetGroupsResponse>(request);
        }

        public Task<KickParticipantsResponse> KickParticipants(KickParticipantsRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<KickParticipantsResponse>(request);
        }

        public Task<AddParticipantsResponse> AddParticipants(AddParticipantsRequest request)
        {
            return _connectionManager.SendRequestAndWaitResponse<AddParticipantsResponse>(request);
        }
    }
}
