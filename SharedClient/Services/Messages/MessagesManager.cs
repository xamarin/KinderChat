using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities.Ws.Events;
using KinderChat.ServerClient.Entities.Ws.Requests;
using KinderChat.ServerClient.Ws.Exceptions;
using KinderChat.ServerClient.Ws.Proxy;
using KinderChat.Services;
using KinderChat.Services.Messages;

namespace KinderChat
{
    public class MessagesManager 
    {
        private readonly MessagingService messagingService;
        private readonly IMessageRepository messageRepository;
        private readonly ICryptoService cryptoService;
        private readonly IDeviceInfoProvider deviceInfoProvider;

        private const string DecryptionErrorMessage = "[The text was encrypted with a wrong Public Key]";

        public MessagesManager(MessagingService messagingService, 
            ConnectionManager connectionManager,
            IMessageRepository messageRepository, 
            ICryptoService cryptoService, 
            IDeviceInfoProvider deviceInfoProvider)
        {
            this.messagingService = messagingService;
            this.messageRepository = messageRepository;
            this.cryptoService = cryptoService;
            this.deviceInfoProvider = deviceInfoProvider;

            connectionManager.Authenticated += OnAuthenticated;
            messagingService.DeliveryNotification += OnMessageDelivered;
            messagingService.IncomingMessage += msg => OnIncomingMessages(new [] { msg });
            messagingService.SeenNotification += OnMessageSeen;
            EncryptionEnabled = true;
        }

        public bool EncryptionEnabled { get; set; }

        public async void OnAuthenticated(AuthenticationResponse authResponse)
        {
            try
            {
                //resend unsent messages:
                var unsentMessages = await messageRepository.GetUnsentMessagesAsync();
                foreach (var message in unsentMessages)
                {
                    await SendMessageAsync(message);
                }

                //process missed messages:
                OnIncomingMessages(authResponse.MissedMessages);
            }
            catch (Exception ex)
            {
                App.Logger.Report(ex);
            }
        }

        public event EventHandler<MessageEventArgs> MessageArrived = delegate { };

        public event EventHandler<MessageStatusEventArgs> MessageStatusChanged = delegate { };

        public async Task<SendMessageResult> SendMessageAsync(Message message)
        {
            try
            {
                Dictionary<string, byte[]> keys = null;
                string text = message.Text;

                await messageRepository.AddMessageAsync(message);

                if (EncryptionEnabled)
                {
                    var deviceIdAndPublicKeyMap = await deviceInfoProvider.GetUserDevicesAndPublicKeys((int) message.Recipient);
                    if (deviceIdAndPublicKeyMap.Count > 0)
                    {
                        //don't forget current user's other devices
                        var myDevices = await deviceInfoProvider.GetUserDevicesAndPublicKeys(Settings.MyId);
                        foreach (var myDevice in myDevices)
                        {
                            if (myDevice.Key != Settings.UserDeviceId && myDevice.Key != Settings.MyId.ToString())
                                deviceIdAndPublicKeyMap[myDevice.Key] = myDevice.Value;
                        }
                        text = cryptoService.Encrypt(message.Text, deviceIdAndPublicKeyMap, out keys);
                    }
                }

                var request = new SendMessageRequest
                    {
                        ReceiverUserId = message.Recipient,
                        SenderName = Settings.NickName,
                        MessageType = ServerClient.Entities.Ws.Requests.MessageType.Text,
                        MessageToken = message.MessageToken,
                        Thumbnail = message.Thumbnail,
                        GroupId = message.GroupId,
                        Message = text,
                        Keys = keys,
                    };

                var response = await messagingService.SendMessage(request);
                if (response.Error == Errors.SendMessage_ProvideKeysForTheseDevices)
                {
                    foreach (var item in response.MissedDevicesWithPublicKeysToReEncrypt)
                    {
                        await deviceInfoProvider.SavePublicKeyForDeviceId(item.DeviceId, item.UserId, Convert.ToBase64String(item.PublicKey));
                    }
                    //try again
                    return await SendMessageAsync(message);
                }
                if (response.Error == Errors.SendMessage_ReceiversNotFound)
                {
                    return SendMessageResult.ReceiverUnknown;
                }
                if (response.Error == Errors.SendMessage_ReceiverAndSenderAreSame)
                {
                    return SendMessageResult.ReceiverAndSenderAreSame;
                }

                OnMessageSent(message.MessageToken);
                return SendMessageResult.Ok;
            }
            catch (ConnectionException)
            {
                return SendMessageResult.ConnectionError;
            }
            catch (Exception ex)
            {
				App.Logger.Report (ex);
                return SendMessageResult.UnknownError;
            }
        }

        public async Task MarkMessageAsSeen(Message message)
        {
            if (Settings.MyId == message.Sender)
            {
                //multi-device message
                return;
            }

            await messagingService.MarkMessageAsSeen(new MarkMessageAsSeenRequest
                {
                    MessagesAuthor = message.Sender,
                    MessagesSeen = new List<Guid> {message.MessageToken}
                });
            message.Status = MessageStatus.Seen;
            await messageRepository.UpdateMessageStatusAsync(message.MessageToken, message.Status);
        }
        
        private async void OnIncomingMessages(IEnumerable<IncomingMessage> messages)
        {
            var incomingMessages = messages.Where(m => m.GroupId == Guid.Empty /*not supported yet*/).OrderBy(m => m.Time).ToList();
            foreach (var msg in incomingMessages)
            {
                string decryptedText = msg.Text;
                try
                {
                    if (msg.EncryptionKey != null && msg.EncryptionKey.Length > 0)
                    {
                        decryptedText = cryptoService.Decrypt(msg.Text, msg.EncryptionKey);
                    }
                }
                catch (Exception)
                {
                    decryptedText = DecryptionErrorMessage;
                }

                var dbMessage = new Message(msg.MessageToken, msg.EventId, msg.Time, msg.ToUserId, msg.FromUserId, decryptedText, msg.Thumbnail, MessageStatus.Delivered);
                await messageRepository.AddMessageAsync(dbMessage);
                MessageArrived(this, new MessageEventArgs { Message = dbMessage });
            }

            SendMessageAck(incomingMessages.Select(m => m.EventId).ToList());
        }

        private async void OnMessageDelivered(DeliveryNotification notification)
        {
            var changed = await messageRepository.UpdateMessageStatusAsync(notification.MessageToken, MessageStatus.Delivered);
            await messagingService.MessageDeliveredStatusAcknowledge(new MessageDeliveredStatusAcknowledgeRequest { Messages = new List<Guid> { notification.EventId } });
            if (changed)
            {
                MessageStatusChanged(this, new MessageStatusEventArgs { MessageToken = notification.MessageToken, Status = MessageStatus.Delivered});
            }
        }

        private async void OnMessageSeen(SeenNotification notification)
        {
            var changed = await messageRepository.UpdateMessageStatusAsync(notification.MessageToken, MessageStatus.Seen);
            await messagingService.MessageSeenStatusAcknowledge(new MessageSeenStatusAcknowledgeRequest { Messages = new List<Guid> { notification.EventId }});
            if (changed)
            {
                MessageStatusChanged(this, new MessageStatusEventArgs { MessageToken = notification.MessageToken, Status = MessageStatus.Seen });
            }
        }

        private async void OnMessageSent(Guid messageToken)
        {
            var changed = await messageRepository.UpdateMessageStatusAsync(messageToken, MessageStatus.Sent);
            if (changed)
            {
                MessageStatusChanged(this, new MessageStatusEventArgs { MessageToken = messageToken, Status = MessageStatus.Sent });
            }
        }

        private void SendMessageAck(List<Guid> idList)
        {
            if (idList.Any())
            {
                messagingService.MessageReceivedStatusAcknowledge(new MessageReceivedStatusAcknowledgeRequest { Messages = idList });
            }
        }
    }

    public enum SendMessageResult
    {
        Ok,
        ConnectionError,
        ReceiverUnknown,
        ReceiverAndSenderAreSame,
        UnknownError
    }
}
