using System;
using KinderChat.ServerClient.Ws.Events;
using KinderChat.ServerClient.Ws.Proxy;

namespace KinderChat.Services.Messages
{
    public class GroupsManager
    {
        private readonly MessagesManager messagesManager;
        private readonly ConnectionManager connectionManager;
        private readonly GroupChatsService groupChatsService;

        public GroupsManager(MessagesManager messagesManager, ConnectionManager connectionManager, GroupChatsService groupChatsService)
        {
            this.messagesManager = messagesManager;
            this.connectionManager = connectionManager;
            this.groupChatsService = groupChatsService;

            groupChatsService.GroupChanged += OnGroupChanged;
        }

        private void OnGroupChanged(GroupChangedNotification e)
        {
            switch (e.Type)
            {
                case GroupChangedNotification.ChangesType.ParticipantsAdded:
                    //fired when:
                    //XXX added ZZZ to the group (you are part of that group)
                    //XXX added you to the group
                    //XXX created the group with you
                    break;
                case GroupChangedNotification.ChangesType.ParticipantsLeft:
                    //fired when:
                    //XXX kicked ZZZ
                    //XXX kicked you
                    break;
                case GroupChangedNotification.ChangesType.NewName:
                    break;
                case GroupChangedNotification.ChangesType.NewAvatar:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
