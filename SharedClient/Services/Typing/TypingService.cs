using System;
using System.Collections.Generic;
using KinderChat.ServerClient.Entities.Ws.Events;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Proxy;

namespace KinderChat
{
    public class TypingService
    {
        private readonly MessagingService messagingService;
        private readonly Dictionary<long, Action<bool>> typingCallbacks = new Dictionary<long, Action<bool>>();

        public TypingService(MessagingService messagingService)
        {
            this.messagingService = messagingService;
            this.messagingService.IsTypingNotification += OnRemoteIsTyping;
        }

        public void SubscribeOnTyping(long conversationId, Action<bool> typingCallback)
        {
            typingCallbacks[conversationId] = typingCallback;
        }

        public void SendIsTyping(bool isTyping, Guid groupId, List<string> devices)
        {
            messagingService.SendIsTyping(new SendIsTypingRequest { Devices = devices, IsTyping = isTyping });
        }
        
        private void OnRemoteIsTyping(IsTypingNotification notification)
        {
            Notify(notification.SenderUserId, notification.IsTyping);
        }

        private void Notify(long userId, bool value)
        {
            Action<bool> action;
            if (typingCallbacks.TryGetValue(userId, out action))
            {
                action(value);
            }
        }
    }
}
