using System;
using System.Collections.Generic;

namespace KinderChat.ServiceBusShared.Entities
{
    public class GroupChangedEvent : Event
    {
        public Guid GroupId { get; set; }

        /// <summary>
        /// a collection of "XXX added/kicked YYY"
        /// </summary>
        public List<GroupChatParticipant> Participants { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public ChangesType Type { get; set; }

        public long ChangesAuthorId { get; set; }

        public enum ChangesType
        {
            ParticipantsAdded,
            ParticipantsLeft,
            NewName,
            NewAvatar
        }
    }

}
