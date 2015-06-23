using System;
using System.Collections.Generic;
using KinderChat.ServerClient.Ws.Entities;

namespace KinderChat.ServerClient.Ws.Events
{
    public class GroupChangedNotification : PushedEvent
    {
        public Guid GroupId { get; set; }

        /// <summary>
        /// a collection of "XXX added/kicked YYY"
        /// XXX kicked XXX means XXX has left the group
        /// </summary>
        public List<GroupChatParticipant> Participants { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public ChangesType Type { get; set; }

        public long ChangesAuthorId { get; set; }

        public enum ChangesType
        {
            ParticipantsAdded, //also it may mean that you've benn added to a group
            ParticipantsLeft, //also it may mean that you've been kicked from the group
            NewName,
            NewAvatar
        }
    }
}
