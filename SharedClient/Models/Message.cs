using System;
using SQLite;

namespace KinderChat
{
	public class Message : EntityBase
    {
		public Message()
		{
		}

        //To be removed:
		public Message (string recipientName, string text)
		{
			RecipientName = recipientName;
			Text = text;
			Date = DateTime.UtcNow;
			MessageToken = Guid.Empty;
		}

	    public Message(Guid messageToken, Guid messageId, DateTime date, long receiverId, long senderId, string text, byte[] thumbnail, MessageStatus status, MessageType type = MessageType.Text)
	    {
	        Recipient = receiverId;
	        Sender = senderId;
	        Text = text;
	        Thumbnail = thumbnail;
            MessageId = messageId;
			MessageToken = messageToken;
	        Date = date;
	        Type = type;
	        Status = status;
	    }

	    public string Text { get; set; }

	    public byte[] Thumbnail { get; set; }

        public MessageType Type { get; set; }

        [Indexed]
        public MessageStatus Status { get; set; }

		public bool IsDelivered {
			get {
				return Status == MessageStatus.Delivered;
			}
		}

		// Identities
	    [Indexed]
	    public long Sender { get; set; }

	    [Indexed]
        public long Recipient { get; set; }

        /// <summary>
        /// Unique Id of the message+device assigned on the server
        /// </summary>
        [Indexed]
        public Guid MessageId { get; set; }

        /// <summary>
        /// Unique Id of the message (but same for all devices) assigned on the server
        /// </summary>
	    [Indexed] 
	    public Guid MessageToken { get; set; }

		// This is computed. Should be ignored during server comunication
		[Ignore]
		public long ConversationId 
		{
			get {
				return Sender ^ Recipient;
			}
		}

		//for now
		public string RecipientName { get; set; }

		// In Ticks (stored in database as ticks already)
		public DateTime Date { get; set; }

		[Ignore]
		public string FriendPhoto {
			get;
			set;
		}

		[Ignore]
		public string FriendName {
			get;
			set;
		}

        [Indexed] 
	    public Guid GroupId { get; set; }
    }
}
