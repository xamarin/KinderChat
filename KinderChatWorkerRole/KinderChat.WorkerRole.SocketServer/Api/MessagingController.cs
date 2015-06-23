using System;
using System.Collections.Generic;
using System.Linq;
using KinderChat.ServerClient.Entities.Ws.Events;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Entities;
using KinderChat.ServiceBusShared;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Api.Base;
using KinderChat.WorkerRole.SocketServer.Api.Base.EventManagement;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;
using ClientDto = KinderChat.ServerClient.Entities.Ws;
using DeliveryNotification = KinderChat.ServiceBusShared.Entities.DeliveryNotification;
using SeenNotification = KinderChat.ServiceBusShared.Entities.SeenNotification;

namespace KinderChat.WorkerRole.SocketServer.Api
{
    public class MessagingController : BaseController
    {
        private readonly IGroupChatsRepository _groupChatsRepository;
        private readonly IDevicesRepository _devicesRepository;
        private readonly ProcessedMessagesQueue _processedMessagesQueue;
        private readonly ReliableEventManager<Message, IncomingMessage> _messageEventManager;
        private readonly ReliableEventManager<DeliveryNotification, ServerClient.Entities.Ws.Events.DeliveryNotification> _deliveryStatusEventManager;
        private readonly ReliableEventManager<SeenNotification, ServerClient.Entities.Ws.Events.SeenNotification> _seenStatusEventManager;
        private readonly InstantEventManager<IsTypingEvent, IsTypingNotification> _isTypingEventManager;

        public MessagingController(
            IGlobalSessionsRegistry sessionsRegistry,
            IGroupChatsRepository groupChatsRepository,
            IDevicesRepository devicesRepository,
            ProcessedMessagesQueue processedMessagesQueue,
            ISessionsServer server,
            IUndeliveredEventsRepository<Message> undeliveredMessagesRepository,
            IUndeliveredEventsRepository<DeliveryNotification> undeliveredDeliveryNotificationsRepository,
            IUndeliveredEventsRepository<SeenNotification> undeliveredSeenNotificationsRepository,
            IInternalMessageBus internalMessageBus)
        {
            _groupChatsRepository = groupChatsRepository;
            _devicesRepository = devicesRepository;
            _processedMessagesQueue = processedMessagesQueue;

            _messageEventManager = new ReliableEventManager<Message, IncomingMessage>(server,
                undeliveredMessagesRepository, sessionsRegistry,internalMessageBus,MessageDtoConverter, OnMessageProcessed);
            RegisterPulsable(_messageEventManager);

            _deliveryStatusEventManager = new ReliableEventManager<DeliveryNotification, ServerClient.Entities.Ws.Events.DeliveryNotification>(server,
                undeliveredDeliveryNotificationsRepository, sessionsRegistry, internalMessageBus,
                n => new ServerClient.Entities.Ws.Events.DeliveryNotification { MessageToken = n.MessageToken, EventId = n.EventId, DeliveredAt = n.CreatedAt });
            RegisterPulsable(_deliveryStatusEventManager);

            _seenStatusEventManager = new ReliableEventManager<SeenNotification, ServerClient.Entities.Ws.Events.SeenNotification>(server, 
                 undeliveredSeenNotificationsRepository, sessionsRegistry, internalMessageBus,
                n => new ServerClient.Entities.Ws.Events.SeenNotification { MessageToken = n.MessageToken, EventId = n.EventId, SeenAt = n.CreatedAt });
            RegisterPulsable(_seenStatusEventManager);

            _isTypingEventManager = new InstantEventManager<IsTypingEvent, IsTypingNotification>(server,
                sessionsRegistry, internalMessageBus, n => new IsTypingNotification { IsTyping = n.IsTyping, SenderUserId = n.SenderUserId, GroupId = n.GroupId });
            RegisterPulsable(_isTypingEventManager);
        }

        [ApiMethod]
        public MarkMessageAsSeenResponse MarkMessageAsSeen(ISession session, MarkMessageAsSeenRequest request)
        {
            foreach (var messageId in request.MessagesSeen)
            {
                Logger.Debug("MarkMessageAsSeen msgToken={0}  from={1}  to={2}", messageId.ToString().Cut(), session.DeviceId.Cut(), request.MessagesAuthor);
                foreach (var deviceId in _devicesRepository.GetDevices(request.MessagesAuthor))
                {
                    _seenStatusEventManager.DeliverEventToDevice(new SeenNotification { MessageToken = messageId, ReceiverDeviceId = deviceId });
                }
            }
            return request.CreateResponse<MarkMessageAsSeenResponse>();
        }

        [ApiMethod]
        public void MessageDeliveredStatusAcknowledge(ISession session, MessageDeliveredStatusAcknowledgeRequest request)
        {
            Logger.Debug("MessageDeliveredStatusAcknowledge from {0} for {1}", session.DeviceId.Cut(), string.Join(", ", request.Messages.Select(i => i.ToString().Cut())));
            _deliveryStatusEventManager.AcknowledgeEvent(session.DeviceId, request.Messages);
        }

        [ApiMethod]
        public void MessageReceivedStatusAcknowledge(ISession session, MessageReceivedStatusAcknowledgeRequest request)
        {
            Logger.Debug("MessageReceivedStatusAcknowledge from {0} for {1}", session.DeviceId.Cut(), string.Join(", ", request.Messages.Select(i => i.ToString().Cut())));
            _messageEventManager.AcknowledgeEvent(session.DeviceId, request.Messages);
        }

        [ApiMethod]
        public void MessageSeenStatusAcknowledge(ISession session, MessageSeenStatusAcknowledgeRequest request)
        {
            Logger.Debug("MessageSeenStatusAcknowledge from {0} for {1}", session.DeviceId.Cut(), string.Join(", ", request.Messages.Select(i => i.ToString().Cut())));
            _seenStatusEventManager.AcknowledgeEvent(session.DeviceId, request.Messages);
        }

        [ApiMethod]
        public void SendIsTyping(ISession session, SendIsTypingRequest request)
        {
            Logger.Trace("IsTyping={0} from {1}", request.IsTyping, session.UserId);
            //actually we may send IsTyping only to "Active" device only, by Active I mean the most recently used one.
            foreach (var deviceId in request.Devices)
            {
                _isTypingEventManager.DeliverEventToDevice(new IsTypingEvent
                    {
                        IsTyping = request.IsTyping,
                        SenderUserId = session.UserId,
                        GroupId = request.GroupId,
                        ReceiverDeviceId = deviceId
                    });
            }
        }

        [ApiMethod]
        public SendMessageResponse SendMessage(ISession session, SendMessageRequest request)
        {
            var response = request.CreateResponse<SendMessageResponse>();
            Logger.Info("SendMessage from Id={0} (Device={1}) to Id={2} ({3} devices)",
                session.UserId, session.DeviceId.Cut(), request.ReceiverUserId, request.Keys != null ? request.Keys.Count : -1);

            Dictionary<string, long> actualMessageReceiversDevices = null;
            Dictionary<string, byte[]> keys = request.Keys;

            if (request.GroupId != Guid.Empty)// group conversation
            {
                var groupChat = _groupChatsRepository.GetChat(request.GroupId);

                //sender is in the group?
                if (groupChat.Participants.All(i => i.UserId != session.UserId))
                {
                    response.Success = false;
                    response.Error = Errors.YouAreNotParticipantOfThatGroup;
                    return response;
                }

                if (keys == null) //means not encrypted
                {
                    keys = groupChat.Participants
                        .SelectMany(p => p.Devices)
                        .Where(p => p != session.DeviceId)
                        .Distinct()
                        .ToDictionary<string, string, byte[]>(k => k, v => null); //null value means unencrypted
                }
                else
                {
                    //it means we will check all provided devices below (see if (actualMessageReceiversDevices != null)) -- just to avoid copy-paste

                    actualMessageReceiversDevices = new Dictionary<string, long>();
                    foreach (var groupChatParticipant in groupChat.Participants)
                    {
                        foreach (var device in groupChatParticipant.Devices)
                        {
                            actualMessageReceiversDevices[device] = groupChatParticipant.UserId;
                        }
                    }
                }
            }
            else // private conversation
            {
                if (request.ReceiverUserId == session.UserId)
                {
                    response.Success = false;
                    response.Error = Errors.SendMessage_ReceiverAndSenderAreSame;
                    return response;
                }

                if (keys == null)//means not encrypted
                {
                    keys = _devicesRepository.GetDevices(request.ReceiverUserId).ToDictionary<string, string, byte[]>(k => k, v => null);
                }
                else
                {
                    //it means we will check all provided devices below (see if (actualMessageReceiversDevices != null)) -- just to avoid copy-paste
                    actualMessageReceiversDevices = _devicesRepository.GetDevices(request.ReceiverUserId).ToDictionary(k => k, v => request.ReceiverUserId);
                }
            }

            if (actualMessageReceiversDevices != null) //TODO: uncoment later
            {
                var providedDevices = keys.Select(i => i.Key);

                List<string> union;
                List<string> notProvidedDevices; //conversation has more devices than user provided as targets
                List<string> wrongDevices; //some of the devices user provided are not members of that conversation
                actualMessageReceiversDevices.Select(i => i.Key)
                    .FindIntersectionAndDifference(providedDevices, out union, out notProvidedDevices, out wrongDevices);

                if (notProvidedDevices.Any())
                {
                    response.Success = false;
                    response.Error = Errors.SendMessage_ProvideKeysForTheseDevices;
                    //we will help the user - provide public keys as well
                    response.MissedDevicesWithPublicKeysToReEncrypt = notProvidedDevices
                       .Where(d => session.DeviceId != d)
                       .Select(d => new PublicKeyInfo
                       {
                           DeviceId = d, 
                           PublicKey = _devicesRepository.GetPublicKeyForDevice(d),
                           UserId = actualMessageReceiversDevices[d]
                       })
                       .ToList();
                    return response;
                }

                //cut receivers not in the conversation
                //TODO: uncomment it later
                /*foreach (var key in wrongDevices)
                {
                    keys.Remove(key);
                }*/
            }

            keys.Remove(session.DeviceId);

            if (keys.Count < 1)
            {
                response.Success = false;
                response.Error = Errors.SendMessage_ReceiversNotFound;
                return response;
            }

            foreach (var key in keys)
            {
                var innerMsg = new Message
                {
                    SenderAccessToken = session.AccessToken,
                    Text = request.Message,
                    GroupId = request.GroupId,
                    SenderId = session.UserId,
                    SenderDeviceId = session.DeviceId,
                    SenderName = request.SenderName,
                    ReceiverId = request.ReceiverUserId,
                    MessageTypeId = (int)request.MessageType,
                    Thumbnail = request.Thumbnail,
                    ReceiverDeviceId = key.Key,
                    EncryptionKey = key.Value,
                    MessageToken = request.MessageToken,
                };
                _messageEventManager.DeliverEventToDevice(innerMsg);
            }
            return response;
        }


        private void OnMessageProcessed(Message msg, bool isSent)
        {
            msg.IsSent = isSent;
            _processedMessagesQueue.Enqueue(msg);
            if (isSent)
            {
                //TODO: remove this line later
                _deliveryStatusEventManager.DeliverEventToDevice(new DeliveryNotification { MessageToken = msg.MessageToken, ReceiverDeviceId = msg.SenderDeviceId });

                foreach (var senderDeviceId in _devicesRepository.GetDevices(msg.SenderId))
                {
                    if (senderDeviceId != msg.SenderDeviceId)
                        _deliveryStatusEventManager.DeliverEventToDevice(new DeliveryNotification { MessageToken = msg.MessageToken, ReceiverDeviceId = senderDeviceId });
                }
            }
        }

        //Use AutoMapper?
        private IncomingMessage MessageDtoConverter(Message msg)
        {
            return new IncomingMessage
            {
                Text = msg.Text,
                GroupId = msg.GroupId,
                EncryptionKey = msg.EncryptionKey,
                Thumbnail = msg.Thumbnail,
                MessageToken = msg.MessageToken,
                FromUserName = msg.SenderName,
                FromUserId = msg.SenderId,
                ToUserId = msg.ReceiverId,
                MessageType = (MessageType)msg.MessageTypeId,
                EventId = msg.EventId,
                Time = msg.CreatedAt
            };
        }

        internal override void OnAuthenticating(ISession session, AuthenticationResponse respone)
        {
            respone.MissedMessages = _messageEventManager
                .GetMissedEvents(session.DeviceId)
                .Select(MessageDtoConverter)
                .ToList();
        }

        internal override void OnAuthenticated(ISession session)
        {
            _deliveryStatusEventManager.DeliverMissedEvents(session);
            _seenStatusEventManager.DeliverMissedEvents(session);
        }
    }
}
