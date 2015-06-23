using System;
using System.Threading;
using System.Threading.Tasks;

namespace KinderChat
{

    /// <summary>
    /// User sends IsTyping with some interval (not for each input char) let's additionally set a timeout for IsTyping=false 
    /// (if server doesn't send any IsTyping flag within this interval - mark it as stopped typing)
    /// </summary>
    internal class OpponentTypingController
    {
        private readonly IUIThreadDispacher threadDispacher;
        private readonly TypingService typingService;
        private Action<bool> isTypingChangedCallback = delegate { };
        private CancellationTokenSource typingCancellationTokenSource = new CancellationTokenSource();

        public static readonly TimeSpan IsNotTypingInterval = TimeSpan.FromSeconds(12);

        public OpponentTypingController(IUIThreadDispacher threadDispacher, TypingService typingService)
        {
            this.threadDispacher = threadDispacher;
            this.typingService = typingService;
        }

        public void Subscribe(long opponentId, Action<bool> isTypingChangedCallback)
        {
            this.isTypingChangedCallback = isTypingChangedCallback;
            typingService.SubscribeOnTyping(opponentId, i => threadDispacher.Dispatch(() => OnTyping(i)));
        }

        public void HandleIncomingMessage()
        {
            OnTyping(false);
        }

        private async void OnTyping(bool isTyping)
        {
            typingCancellationTokenSource.Cancel();
            typingCancellationTokenSource = new CancellationTokenSource();

            isTypingChangedCallback(isTyping);
            if (isTyping)
            {
                try
                {
                    await Task.Delay(IsNotTypingInterval, typingCancellationTokenSource.Token);
                    isTypingChangedCallback(false);
                }
                catch (OperationCanceledException) {}
            }
        }
    }
}
