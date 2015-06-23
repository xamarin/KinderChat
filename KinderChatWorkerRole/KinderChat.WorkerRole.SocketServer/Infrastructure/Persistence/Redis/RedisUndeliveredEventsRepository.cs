using System;
using System.Collections.Generic;
using System.Linq;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;
using StackExchange.Redis;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.Redis
{
    public class RedisUndeliveredEventsRepository<TEvent> : IUndeliveredEventsRepository<TEvent> where TEvent : Event
    {
        private readonly IDatabase cache;
        private static readonly string KeyPrefix = "ruer_" + typeof(TEvent).Name;

        public RedisUndeliveredEventsRepository(string redisConnectionString)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(redisConnectionString);//set "Allow access only via SSL" to false, we don't need that overhead
            cache = connection.GetDatabase();
        }

        public List<TEvent> GetAll(string receiverDeviceId)
        {
            var items = cache.ListRange(KeyPrefix + receiverDeviceId) ?? new RedisValue[0];
            return items.Select(i => RedisExtensions.Deserialize<TEvent>(i)).ToList();
        }

        public void Add(TEvent e)
        {
            cache.ListRightPush(KeyPrefix + e.ReceiverDeviceId, RedisExtensions.Serialize(e), flags: CommandFlags.FireAndForget);
        }


        public List<TEvent> DeleteAll(string deviceId, IEnumerable<Guid> messagesIds)
        {
            /*var transaction = cache.CreateTransaction();
            HashSet<Guid> idSet = new HashSet<Guid>(messagesIds);
            var items = (await transaction.ListRangeAsync(deviceId).ConfigureAwait(false)) ?? new RedisValue[0];
            
            if (items.Length < 1) //nothing to remove, it's empty
            {
                await transaction.ExecuteAsync();
                return;
            }
            
            var notAckMessagesYet = new List<RedisValue>(items.Length);
            foreach (var item in items) 
            {
                var msg = RedisExtensions.Deserialize<Message>(item);
                if (!idSet.Contains(msg.MessageId))
                {
                    notAckMessagesYet.Add(item);
                }
            }

            if (notAckMessagesYet.Count == items.Length) //it's not empty but nothing to remove.
            {
                await transaction.ExecuteAsync();
                return;
            }

            //delete the key and fill it again with a new list
            await transaction.KeyDeleteAsync(deviceId).ConfigureAwait(false);
            await transaction.ListRightPushAsync(deviceId, notAckMessagesYet.ToArray()).ConfigureAwait(false);
            await transaction.ExecuteAsync().ConfigureAwait(false);*/

            HashSet<Guid> idSet = new HashSet<Guid>(messagesIds);
            var items = cache.ListRange(KeyPrefix + deviceId) ?? new RedisValue[0];

            if (items.Length < 1) //nothing to remove, it's empty
            {
                return new List<TEvent>(0);
            }

            var notAckMessagesYet = new List<RedisValue>(items.Length);
            foreach (var item in items)
            {
                var msg = RedisExtensions.Deserialize<Event>(item);
                if (!idSet.Contains(msg.EventId))
                {
                    notAckMessagesYet.Add(item);
                }
            }

            if (notAckMessagesYet.Count == items.Length) //it's not empty but nothing to remove.
            {
                return new List<TEvent>(0);
            }

            //delete the key and fill it again with a new list
            cache.KeyDelete(KeyPrefix + deviceId);
            cache.ListRightPush(KeyPrefix + deviceId, notAckMessagesYet.ToArray(), flags: CommandFlags.FireAndForget);
            return notAckMessagesYet.Select(i => RedisExtensions.Deserialize<TEvent>(i)).ToList();
        }
    }
}
