using System;
using System.Collections.Generic;
using KinderChat.ServerClient.Ws.Entities;

namespace KinderChat.ServerClient.Entities.Ws.Requests
{
    public class SendMessageRequest : BaseRequest
    {
        public Guid GroupId { get; set; }

        public Guid MessageToken { get; set; }

        public long ReceiverUserId { get; set; }

        /// <summary>
        /// Dictionary of:
        /// {DeviceId} -- {Encrypted with RSA an AES key}
        /// </summary>
        public Dictionary<string, byte[]> Keys { get; set; }

        /// <summary>
        /// Message
        /// If Keys list is not empty - it means that the message is encrypted with AES
        /// </summary>
        public string Message { get; set; }
        
        public string SenderName { get; set; }

        public MessageType MessageType { get; set; }
        
        public byte[] Thumbnail { get; set; }
    }

    public enum MessageType
    {
        Text = 0,
        Image,
        //most of IM have these types:
        Video,
        File,
        Voice,
        Location,
        Sticker
    }


    public class SendMessageResponse : BaseResponse
    {
        /// <summary>
        /// Server sent you a map of deviceId-public_key to re encrypt your message - as you don't really know all devices of the receivers
        /// </summary>
        public List<PublicKeyInfo> MissedDevicesWithPublicKeysToReEncrypt { get; set; }
    }
}
