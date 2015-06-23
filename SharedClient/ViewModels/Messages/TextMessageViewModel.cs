namespace KinderChat.ViewModels.Messages
{
    public class TextMessageViewModel : MessageViewModel
    {
        private string text;

        public TextMessageViewModel(Message message)
            : base(message)
        {
        }

        protected override void OnUnderlyingMessageChanged(Message msg)
        {
            Text = msg.Text;
        }

        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }
    }
}
