using System;
using System.Collections.Generic;
using System.Linq;
using Console;
using KinderChat.ServerClient.Entities.Ws.Events;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Events;
using KinderChat.ServerClient.Ws.Proxy;
using KinderChat.ServerClient.Ws.Requests;
using Out = System.Console; //because of namespace Console

namespace KinderChat
{
	class MainClass
	{
        private static bool authenticated = false;
	    private static ConnectionManager connectionManager;
	    private static MessagingService messagingService;
	    private static GroupChatsService groupChatsService;
        private static long myUserId;
	    private static string myDeviceId;

	    public static void Main (string[] args)
	    {
            Out.WriteLine("Enter Your Id:");
            myUserId = long.Parse(Out.ReadLine());
            Out.WriteLine("Enter Your DeviceId:");
	        myDeviceId = Out.ReadLine();

	        StartChatting();

	        while (true)
	        {
	            var line = Out.ReadLine();
                if (!authenticated || string.IsNullOrEmpty(line))
                    continue;
	            ProcessConsoleCommand(line);
	        }
		}

	    private static async void ProcessConsoleCommand(string line)
        {
	        var args = new List<string>();
            if (CommandParser.ParseCommand(line, "BeginTyping", 1, ref args))
            {
                var receiverId = long.Parse(args[0]);
                await messagingService.SendIsTyping(new SendIsTypingRequest { IsTyping = true, Devices = new List<string> { receiverId.ToString() }});
            }
            else if (CommandParser.ParseCommand(line, "EndTyping", 1, ref args))
            {
                var receiverId = long.Parse(args[0]);
                await messagingService.SendIsTyping(new SendIsTypingRequest { IsTyping = false, Devices = new List<string> { receiverId.ToString() }});
            }
            else if (CommandParser.ParseCommand(line, "CreateGroup", -1, ref args))
            {
                var participants = args
                    .Select(long.Parse)
                    .ToList();
                var createGroupChatResponse = await groupChatsService.CreateGroupChat(new CreateGroupChatRequest { GroupName = "Foo", Participants = participants});
                Out.WriteLine("Group is created: {0}", createGroupChatResponse.GroupId);
            }
            else if (CommandParser.ParseCommand(line, "AddParticipant", -1, ref args))
            {
                var groupId = Guid.Parse(args[0]);
                await groupChatsService.AddParticipants(new AddParticipantsRequest { GroupId = groupId, ParticipantIds = args.Skip(1).Select(long.Parse).ToList()});
            }
            else if (CommandParser.ParseCommand(line, "KickParticipant", -1, ref args))
            {
                var groupId = Guid.Parse(args[0]);
                await groupChatsService.KickParticipants(new KickParticipantsRequest { GroupId = groupId, ParticipantIds = args.Skip(1).Select(long.Parse).ToList() });
            }
            else if (CommandParser.ParseCommand(line, "LeaveGroup", 1, ref args))
            {
                var groupId = Guid.Parse(args[0]);
                await groupChatsService.LeaveGroup(new LeaveGroupRequest {GroupId = groupId});
            }
            else if (CommandParser.ParseCommand(line, "SetGroupName", 2, ref args))
            {
                var groupId = Guid.Parse(args[0]);
                await groupChatsService.ChangeGroup(new ChangeGroupRequest {GroupId = groupId, NewGroupName = args[1]});
            }
            else if (CommandParser.ParseCommand(line, "GetGroupInfo", 1, ref args))
            {
                var groupId = Guid.Parse(args[0]);
                var getGroupChatInfoResponse = await groupChatsService.GetGroupChatInfo(new GetGroupChatInfoRequest {GroupId = groupId});
                Out.WriteLine("GetGroupInfo: Name={0}, Participants.Count={1}", getGroupChatInfoResponse.GroupInfo.Name, getGroupChatInfoResponse.GroupInfo.Participants.Count);
            }
            else if (CommandParser.ParseCommand(line, "GetMyGroups", 0, ref args))
            {
                var getGroupsResponse = await groupChatsService.GetGroups(new GetGroupsRequest());
                Out.WriteLine("GetMyGroup: Groups.Count={0}", getGroupsResponse.Groups);
            }
            //private message
            else if (CommandParser.ParseCommand(line, "SendMessage", 2, ref args))
            {
                var receiverId = long.Parse(args[0]);
                string text = args[1];
                var sendMsgRequest = new SendMessageRequest
                    { 
                        ReceiverUserId = receiverId,
                        Message = text,
                        //Keys = new Dictionary<string, byte[]> { { receiverId.ToString(), new byte[1] } },
                        MessageToken = Guid.NewGuid(),
                        SenderName = "ConsoleAppFor_" + myUserId
                    };
                await messagingService.SendMessage(sendMsgRequest);
                Out.WriteLine("   server received your message");
            }
            //group message
            else if (CommandParser.ParseCommand(line, "SendGroupMessage", 2, ref args))
            {
                var groupId = Guid.Parse(args[0]);
                string text = args[1];
                var sendMsgRequest = new SendMessageRequest
                {
                    ReceiverUserId = 0,
                    GroupId = groupId,
                    Message = text,
                    //Keys = new Dictionary<string, byte[]> { { receiverId.ToString(), new byte[1] } },
                    MessageToken = Guid.NewGuid(),
                    SenderName = "ConsoleAppFor_" + myUserId
                };
                await messagingService.SendMessage(sendMsgRequest);
                Out.WriteLine("   server received your message");
            }
            else
            {
                Out.WriteLine("Command is not recognized");
            }
	    }

	    private static async void StartChatting()
	    {
            connectionManager = new ConnectionManager(new WebSocketConnection(),
                new AdhocCredentialsProvider { AccessToken = "fake", DeviceId = myDeviceId, UserId = myUserId, PublicKey = new byte[] { 1, 2, 3 } });

            messagingService = new MessagingService(connectionManager);
            groupChatsService = new GroupChatsService(connectionManager);


	        connectionManager.Authenticated += OnAuthenticated;
            groupChatsService.GroupChanged += GroupChatsServiceOnGroupChanged;
            messagingService.IncomingMessage += MessagingServiceOnIncomingMessage;
            messagingService.IsTypingNotification += MessagingServiceOnIsTypingNotification;
            messagingService.DeliveryNotification += MessagingServiceOnDeliveryNotification;
            messagingService.SeenNotification += MessagingServiceOnSeenNotification;

	        await connectionManager.TryKeepConnectionAsync();
	    }

	    private static void GroupChatsServiceOnGroupChanged(GroupChangedNotification e)
        {
            Out.WriteLine("     GroupChangedNotification: {0}, Name:{1}", e.Type, e.Name);
	    }

	    private async static void OnAuthenticated(AuthenticationResponse authResponse)
        {

            Out.WriteLine("Authenticated. Missed messages:");
            foreach (var msg in authResponse.MissedMessages)
            {
                Out.WriteLine(">> {0}", msg.Text);
            }
            await messagingService.MessageReceivedStatusAcknowledge(
                new MessageReceivedStatusAcknowledgeRequest
                {
                    Messages = authResponse
                        .MissedMessages
                        .Select(msg => msg.EventId)
                        .ToList()
                });
            authenticated = true;
	    }

	    private static async void MessagingServiceOnSeenNotification(SeenNotification notification)
        {
            Out.WriteLine("   opponent saw your message");
            await messagingService.MessageSeenStatusAcknowledge(
                new MessageSeenStatusAcknowledgeRequest { Messages = new List<Guid> { notification.EventId } });
	    }

        private static async void MessagingServiceOnDeliveryNotification(DeliveryNotification notification)
        {
            Out.WriteLine("   opponent received your message");
            await messagingService.MessageDeliveredStatusAcknowledge(
                new MessageDeliveredStatusAcknowledgeRequest { Messages = new List<Guid> { notification.EventId }});
	    }

        private static void MessagingServiceOnIsTypingNotification(IsTypingNotification notification)
        {
            Out.WriteLine("   opponent({1}) sent 'IsTyping' event ({0})", notification.IsTyping, notification.SenderUserId);
	    }

        private static async void MessagingServiceOnIncomingMessage(IncomingMessage notification)
        {
            Out.WriteLine(">> {0}", notification.Text);

            await messagingService.MessageReceivedStatusAcknowledge(
                new MessageReceivedStatusAcknowledgeRequest { Messages = new List<Guid> { notification.EventId } });

            await messagingService.MarkMessageAsSeen(
                new MarkMessageAsSeenRequest { MessagesSeen = new List<Guid> { notification.EventId }, MessagesAuthor = notification.FromUserId });
	    }
	}
}
