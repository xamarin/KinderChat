using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.InMemory
{
    public class InMemoryGroupChatsRepository : IGroupChatsRepository
    {
        private readonly ConcurrentDictionary<Guid, GroupChat> _chats = new ConcurrentDictionary<Guid, GroupChat>();
        private readonly ConcurrentDictionary<long, List<Guid>> _groupsByUsers = new ConcurrentDictionary<long, List<Guid>>();

        public GroupChat GetChat(Guid id)
        {
            return _chats[id];
        }

        public void UpdateOrCreateGroup(GroupChat chat)
        {
            _chats[chat.GroupId] = chat;
        }

        public List<Guid> GetGroupsForUser(long userId)
        {
            return _groupsByUsers[userId];
        }

        public void AddGroupToUser(long userId, Guid groupId)
        {
            _groupsByUsers.ChangeListFromDictionary(userId, list => list.Add(groupId));
        }

        public void RemoveGroupFromUser(long userId, Guid groupId)
        {
            _groupsByUsers.ChangeListFromDictionary(userId, list => list.Remove(groupId));
        }
    }
}