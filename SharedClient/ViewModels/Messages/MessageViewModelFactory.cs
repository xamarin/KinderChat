namespace KinderChat.ViewModels.Messages
{
    public static class MessageViewModelFactory
    {
        public static MessageViewModel Create(Message msg)
        {
            if (msg.Thumbnail != null && msg.Thumbnail.Length > 0)
            {
                return new ImageMessageViewModel(msg);
            }
            return new TextMessageViewModel(msg);
        }
    }
}
