using System;
using System.Collections.Generic;
using System.Linq;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;
using StackExchange.Redis;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.Redis
{
    public class RedisGroupChatsRepository : IGroupChatsRepository
    {
        private readonly IDatabase _cache;
        private const string GroupsKeyPrefix = "rgcr_";
        private const string UsersGroupsKeyPrefix = "ugs_";

        public RedisGroupChatsRepository(string connectionString)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(connectionString);
            _cache = connection.GetDatabase();
        }

        public GroupChat GetChat(Guid id)
        {
            return _cache.Get<GroupChat>(GroupsKeyPrefix + id.ToString());
        }

        public void UpdateOrCreateGroup(GroupChat chat)
        {
            _cache.Set(GroupsKeyPrefix + chat.GroupId.ToString(), RedisExtensions.Serialize(chat));
        }

        public List<Guid> GetGroupsForUser(long userId)
        {
            return _cache.ListRange(UsersGroupsKeyPrefix + userId.ToString()).Select(i => new Guid(i.ToString())).ToList();
        }

        public void AddGroupToUser(long userId, Guid groupId)
        {
            _cache.ListRightPush(UsersGroupsKeyPrefix + userId, groupId.ToString(), flags: CommandFlags.FireAndForget);
        }

        public void RemoveGroupFromUser(long userId, Guid groupId)
        {
            _cache.ListRemove(UsersGroupsKeyPrefix + userId, groupId.ToString(), flags: CommandFlags.FireAndForget);
        }
    }
}