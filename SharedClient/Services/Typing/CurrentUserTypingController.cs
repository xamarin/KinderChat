using System;
using System.Threading;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities.Ws;
using KinderChat.Services;

namespace KinderChat
{
    /// <summary>
    /// It's all about not to send IsTyping flag for each input char but use some intervals 
    /// </summary>
    public class CurrentUserTypingController
    {
        public static readonly TimeSpan ClearingInterval = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan TypingInterval = TimeSpan.FromSeconds(6);//

        private CancellationTokenSource typingCancellationTokenSource = new CancellationTokenSource();
        private readonly long opponentId;
        private readonly TypingService _typingService;
        private readonly IDeviceInfoProvider deviceInfoProvider;
        private DateTime lastTypingAt = DateTime.MinValue;
        private bool isTyping = false;

        public CurrentUserTypingController(long opponentId, TypingService typingService, IDeviceInfoProvider deviceInfoProvider)
        {
            this.opponentId = opponentId;
            this._typingService = typingService;
            this.deviceInfoProvider = deviceInfoProvider;
        }

        /// <summary>
        /// User erased input text
        /// </summary>
        public async void HandleClear()
        {
            RenewTyping();
            if (await Delay(ClearingInterval))
            {
                ChangeStatus(false);
            }
        }

        /// <summary>
        /// User changed input text (call it for each change)
        /// </summary>
        public async void HandleTyping()
        {
            RenewTyping();

            if (isTyping && (DateTime.UtcNow - lastTypingAt) > TypingInterval)
            {
                lastTypingAt = DateTime.UtcNow;
                ChangeStatus(true);
            }
            else if (!isTyping)
            {
                lastTypingAt = DateTime.UtcNow;
                ChangeStatus(true);
            }
            if (await Delay(TypingInterval))
            {
                if (isTyping && (DateTime.UtcNow - lastTypingAt) > TypingInterval)
                {
                    ChangeStatus(false);
                }
            }
        }

        /// <summary>
        /// SendMessage means we are not typing anymore
        /// </summary>
        public void HandleOutgoingMessage()
        {
            this.OnStopTyping();
        }

        /// <summary>
        /// Closing chat means we are not typing anymore
        /// </summary>
        public void HandleLeave()
        {
            this.OnStopTyping();
        }

        private void OnStopTyping()
        {
            if (isTyping)
            {
                ChangeStatus(false);
            }
        }

        private async void ChangeStatus(bool isTyping)
        {
            try
            {
                this.isTyping = isTyping;
                var getDevicesStarted = DateTime.UtcNow;
                var list = await deviceInfoProvider.GetUserDevices(opponentId);
                if (DateTime.UtcNow - getDevicesStarted < TypingInterval)
                {
                    _typingService.SendIsTyping(isTyping, Guid.Empty, list);
                }
            }
            catch
            {
                //it doesn't really matter if IsTyping fails
            }
        }

        private async Task<bool> Delay(TimeSpan delay)
        {
            try
            {
                await Task.Delay(delay, typingCancellationTokenSource.Token);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        private void RenewTyping()
        {
            typingCancellationTokenSource.Cancel();
            typingCancellationTokenSource = new CancellationTokenSource();
        }
    }
}
