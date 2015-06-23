using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.InMemory
{
    public class InMemoryUndeliveredEventsRepository<TEvent> : IUndeliveredEventsRepository<TEvent> where TEvent : Event
    {
        private static readonly ConcurrentDictionary<string, List<TEvent>> MessagesDictionary =
            new ConcurrentDictionary<string, List<TEvent>>();

        public List<TEvent> GetAll(string receiverDeviceId)
        {
            var list = GetUndeliveredMessageList(receiverDeviceId);
            lock (list)
            {
                return list.ToList();
            }
        }

        public void Add(TEvent message)
        {
            var list = GetUndeliveredMessageList(message.ReceiverDeviceId);
            lock (list)
            {
                list.Add(message);
            }
        }

        public List<TEvent> DeleteAll(string deviceId, IEnumerable<Guid> messagesIds)
        {
            var list = GetUndeliveredMessageList(deviceId);
            lock (list)
            {
                var result = list.Where(i => messagesIds.Contains(i.EventId)).ToList();
                foreach (var item in result)
                {
                    list.Remove(item);
                }
                return result;
            }
        }

        private List<TEvent> GetUndeliveredMessageList(string receiverDeviceId)
        {
            List<TEvent> list;
            if (MessagesDictionary.TryGetValue(receiverDeviceId, out list))
            {
                return list;
            }
            return MessagesDictionary[receiverDeviceId] = new List<TEvent>();
        }
    }
}