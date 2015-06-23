using System;

namespace KinderChat.ViewModels.Messages
{
    public class MessageViewModel : BaseViewModel
    {
        private bool isIncoming;
        private string authorPhoto;
        private DateTime timestamp;
        private MessageStatus status;
        private string authorName;
        private Message underlyingMessage;

        protected MessageViewModel(Message message)
        {
            UnderlyingMessage = message;
        }

        public string AuthorName
        {
            get { return authorName; }
            set { SetProperty(ref authorName, value); }
        }

        public const string StatusPropertyName = "Status";
        public MessageStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public DateTime Timestamp
        {
            get { return timestamp; }
            set { SetProperty(ref timestamp, value); }
        }

        public string AuthorPhoto
        {
            get { return authorPhoto; }
            set { SetProperty(ref authorPhoto, value); }
        }

        public bool IsIncoming
        {
            get { return isIncoming; }
            set { SetProperty(ref isIncoming, value); }
        }

        public Message UnderlyingMessage
        {
            get { return underlyingMessage; }
            set
            {
                SetProperty(ref underlyingMessage, value);

                IsIncoming = Settings.MyId != underlyingMessage.Sender;
                Timestamp = underlyingMessage.Date;
                AuthorPhoto = underlyingMessage.FriendPhoto;
                AuthorName = underlyingMessage.FriendName;
                Status = underlyingMessage.Status;
                OnUnderlyingMessageChanged(underlyingMessage);
            }
        }

        protected virtual void OnUnderlyingMessageChanged(Message msg) { }

        public void ChangeStatus(MessageStatus status)
        {
            Status = status;
            UnderlyingMessage.Status = status;
        }
    }
}
