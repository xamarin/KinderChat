namespace KinderChat.ViewModels.Messages
{
    public class ImageMessageViewModel : MessageViewModel
    {
        private byte[] thumbnail;

        public ImageMessageViewModel(Message message)
            : base(message)
        {
        }

        protected override void OnUnderlyingMessageChanged(Message msg)
        {
            Thumbnail = msg.Thumbnail;
        }

        public byte[] Thumbnail
        {
            get { return thumbnail; }
            set { SetProperty(ref thumbnail, value); }
        }
    }
}
