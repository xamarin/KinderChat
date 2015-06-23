using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KinderChat.Services.Messages
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetUnsentMessagesAsync();

        Task<bool> AddMessageAsync(Message dbMessage);

        Task<bool> UpdateMessageStatusAsync(Guid messageToken, MessageStatus newStatus);

        Task<List<Message>> GetMessagesAsync(long conversationId);
    }
}
