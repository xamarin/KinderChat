using System;
using System.Collections.Generic;
using System.Linq;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Entities;
using KinderChat.ServerClient.Ws.Events;
using KinderChat.ServerClient.Ws.Requests;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Api.Base;
using KinderChat.WorkerRole.SocketServer.Api.Base.EventManagement;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;
using ClientDto = KinderChat.ServerClient.Ws;
using GroupChatParticipant = KinderChat.ServiceBusShared.Entities.GroupChatParticipant;

namespace KinderChat.WorkerRole.SocketServer.Api
{
    public class GroupChatsController : BaseController
    {
        private readonly IDevicesRepository _devicesRepository;
        private readonly IGroupChatsRepository _groupChatsRepository;
        private readonly ReliableEventManager<GroupChangedEvent, ClientDto.Events.GroupChangedNotification> _groupChangedEventManager;

        public GroupChatsController(ISessionsServer server, 
            IDevicesRepository devicesRepository,
            IUndeliveredEventsRepository<GroupChangedEvent> undeliveredGroupChangedEventsRepository,
            IGroupChatsRepository groupChatsRepository,
            IGlobalSessionsRegistry sessionsRegistry, 
            IInternalMessageBus internalMessageBus)
        {
            _devicesRepository = devicesRepository;
            _groupChatsRepository = groupChatsRepository;
            _groupChangedEventManager = new ReliableEventManager<GroupChangedEvent, ClientDto.Events.GroupChangedNotification>(server,
                undeliveredGroupChangedEventsRepository,
                sessionsRegistry,
                internalMessageBus,
                GroupChangedDtoConverter);

            RegisterPulsable(_groupChangedEventManager);
        }

        //AutoMapper?
        private GroupChangedNotification GroupChangedDtoConverter(GroupChangedEvent entity)
        {
            return new GroupChangedNotification
                {
                    Participants = entity.Participants != null ? entity.Participants
                        .Select(p => new ClientDto.Entities.GroupChatParticipant { Avatar = p.Avatar, Name = p.Name, UserId = p.UserId, Devices = p.Devices })
                        .ToList() : null,
                    ChangesAuthorId = entity.ChangesAuthorId,
                    Avatar = entity.Avatar,
                    Name = entity.Name,
                    GroupId = entity.GroupId,
                    Type = (GroupChangedNotification.ChangesType) (int) entity.Type
                };
        }

        [ApiMethod]
        public ChangeGroupResponse ChangeGroup(ISession session, ChangeGroupRequest request)
        {
            var response = request.CreateResponse<ChangeGroupResponse>();
            var groupChat = _groupChatsRepository.GetChat(request.GroupId);
            if (!HasAccess(groupChat, session.UserId))
            {
                response.Success = false;
                response.Error = Errors.OperationIsNotPermitted;
                return response;
            }

            if (groupChat.Avatar != request.NewGroupAvatar)
            {
                groupChat.Avatar = request.NewGroupAvatar;
                _groupChatsRepository.UpdateOrCreateGroup(groupChat);
                foreach (var participant in groupChat.Participants)
                {
                    _groupChangedEventManager.DeliverEventToDevices(participant.Devices, 
                        () => new GroupChangedEvent
                        {
                            ChangesAuthorId = session.UserId,
                            GroupId = request.GroupId,
                            Avatar = request.NewGroupAvatar,
                            Type = GroupChangedEvent.ChangesType.NewAvatar
                        });
                }
            }

            if (groupChat.Name != request.NewGroupName)
            {
                groupChat.Name = request.NewGroupName;
                _groupChatsRepository.UpdateOrCreateGroup(groupChat);
                foreach (var participant in groupChat.Participants)
                {
                    _groupChangedEventManager.DeliverEventToDevices(participant.Devices,
                        () => new GroupChangedEvent
                        {
                            ChangesAuthorId = session.UserId,
                            GroupId = request.GroupId,
                            Name = request.NewGroupName,
                            Type = GroupChangedEvent.ChangesType.NewName
                        });
                }
            }
            return response;
        }

        [ApiMethod]
        public void GroupChangedAcknowledge(ISession session, GroupChangedAcknowledgeRequest request)
        {
            _groupChangedEventManager.AcknowledgeEvent(session.DeviceId, new List<Guid> { request.EventId });
        }

        [ApiMethod]
        public CreateGroupChatResponse CreateGroupChat(ISession session, CreateGroupChatRequest request)
        {
            var response = request.CreateResponse<CreateGroupChatResponse>();

            GroupChat group = new GroupChat { GroupId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, OwnerId = session.UserId};
            group.Participants = new List<GroupChatParticipant> { new GroupChatParticipant { UserId = session.UserId }};
            group.Participants.AddRange(request.Participants.Select(i => new GroupChatParticipant { UserId = i }));

            foreach (var participant in group.Participants)
            {
                _groupChatsRepository.AddGroupToUser(participant.UserId, group.GroupId);
                participant.Devices = _devicesRepository.GetDevices(participant.UserId);
            }

            foreach (var device in group.Participants.SelectMany(p => p.Devices))
            {
                if (device != session.DeviceId)
                {
                    _groupChangedEventManager.DeliverEventToDevice(new GroupChangedEvent
                        {
                            ReceiverDeviceId = device,
                            ChangesAuthorId = session.UserId,
                            GroupId = group.GroupId,
                            Participants = group.Participants,
                            Type = GroupChangedEvent.ChangesType.ParticipantsAdded
                        });
                }
            }

            _groupChatsRepository.UpdateOrCreateGroup(group);

            response.GroupId = group.GroupId;
            return response;
        } 

        [ApiMethod]
        public GetGroupChatInfoResponse GetGroupChatInfo(ISession session, GetGroupChatInfoRequest request)
        {
            var response = request.CreateResponse<GetGroupChatInfoResponse>();
            var groupChat = _groupChatsRepository.GetChat(request.GroupId);
            if (!HasAccess(groupChat, session.UserId))
            {
                response.Success = false;
                response.Error = Errors.OperationIsNotPermitted;
                return response;
            }
            
            //TODO: AutoMapper
            response.GroupInfo = new GroupInfo
                {
                    Avatar = groupChat.Avatar,
                    GroupId = groupChat.GroupId,
                    CreatedAt = groupChat.CreatedAt,
                    Name = groupChat.Name,
                    OwnerId = groupChat.OwnerId,
                    Participants = groupChat.Participants
                        .Select(p => new ClientDto.Entities.GroupChatParticipant { UserId = p.UserId, Avatar = p.Avatar, Devices = p.Devices, Name = p.Name })
                        .ToList()
                };

            return response;
        }

        [ApiMethod]
        public GetGroupsResponse GetGroups(ISession session, GetGroupsRequest request)
        {
            var response = request.CreateResponse<GetGroupsResponse>();
            var groups = _groupChatsRepository.GetGroupsForUser(session.UserId).Select(id => _groupChatsRepository.GetChat(id));

            //TODO: AutoMapper
            response.Groups = groups.Select(groupChat => 
                new GroupInfo
                {
                    Avatar = groupChat.Avatar,
                    CreatedAt = groupChat.CreatedAt,
                    GroupId = groupChat.GroupId,
                    Name = groupChat.Name,
                    OwnerId = groupChat.OwnerId,
                    Participants = groupChat.Participants
                        .Select(p => new ClientDto.Entities.GroupChatParticipant { UserId = p.UserId, Avatar = p.Avatar, Devices = p.Devices, Name = p.Name })
                        .ToList()
                }).ToList();

            return response;
        }

        [ApiMethod]
        public LeaveGroupResponse LeaveGroup(ISession session, LeaveGroupRequest request)
        {
            KickParticipants(session, new KickParticipantsRequest { GroupId = request.GroupId, ParticipantIds = new List<long>() { session.UserId }});
            return request.CreateResponse<LeaveGroupResponse>();
        }

        [ApiMethod]
        public KickParticipantsResponse KickParticipants(ISession session, KickParticipantsRequest request)
        {
            var response = request.CreateResponse<KickParticipantsResponse>();
            var groupChat = _groupChatsRepository.GetChat(request.GroupId);
            var originalParticipants = groupChat.Participants.ToList();
            if (!HasAccess(groupChat, session.UserId))
            {
                response.Success = false;
                response.Error = Errors.OperationIsNotPermitted;
                return response;
            }

            var leftParticipants = new List<GroupChatParticipant>();
            foreach (var participantId in request.ParticipantIds)
            {
                if (session.UserId != participantId && groupChat.OwnerId != session.UserId)
                {
                    response.Success = false;
                    response.Error = Errors.KickParticipants_OnlyOwnerCanKickParticipants;
                    return response;
                }

                var participant = new GroupChatParticipant { UserId = participantId };
                if (groupChat.Participants.RemoveAll(r => r.UserId == participantId) > 0)
                {
                    leftParticipants.Add(participant);
                    _groupChatsRepository.RemoveGroupFromUser(participantId, request.GroupId);
                }
            }

            foreach (var participant in originalParticipants)
            {
                _groupChangedEventManager.DeliverEventToDevices(participant.Devices,
                    () => new GroupChangedEvent
                    {
                        ChangesAuthorId = session.UserId,
                        GroupId = request.GroupId,
                        Participants = leftParticipants,
                        Type = GroupChangedEvent.ChangesType.ParticipantsLeft
                    });
            }

            if (leftParticipants.Count > 0)
            {
                _groupChatsRepository.UpdateOrCreateGroup(groupChat);
            }

            return response;
        }

        [ApiMethod]
        public AddParticipantsResponse AddParticipants(ISession session, AddParticipantsRequest request)
        {
            var response = request.CreateResponse<AddParticipantsResponse>();
            var groupChat = _groupChatsRepository.GetChat(request.GroupId);
            if (!HasAccess(groupChat, session.UserId))
            {
                response.Success = false;
                response.Error = Errors.OperationIsNotPermitted;
                return response;
            }

            var addedParticipants = new List<GroupChatParticipant>();
            foreach (var participantId in request.ParticipantIds)
            {
                var participant = new GroupChatParticipant { UserId = participantId, Devices = _devicesRepository.GetDevices(participantId) };
                if (groupChat.Participants.AddIfUnique(participant, r => r.UserId))
                {
                    _groupChatsRepository.AddGroupToUser(participant.UserId, request.GroupId);
                    addedParticipants.Add(participant);
                }
            }

            foreach (var participant in addedParticipants)
            {
                _groupChangedEventManager.DeliverEventToDevices(participant.Devices, 
                    () => new GroupChangedEvent
                    {
                        ChangesAuthorId = session.UserId, 
                        GroupId = request.GroupId,
                        Participants = addedParticipants,
                        Type = GroupChangedEvent.ChangesType.ParticipantsAdded
                    });
            }

            if (addedParticipants.Count > 0)
            {
                _groupChatsRepository.UpdateOrCreateGroup(groupChat);
            }

            return response;
        }

        internal override void OnAuthenticated(ISession session)
        {
            _groupChangedEventManager.DeliverMissedEvents(session);
        }

        private bool HasAccess(GroupChat group, long userId)
        {
            return group.Participants.Any(p => p.UserId == userId);
        }
    }
}
