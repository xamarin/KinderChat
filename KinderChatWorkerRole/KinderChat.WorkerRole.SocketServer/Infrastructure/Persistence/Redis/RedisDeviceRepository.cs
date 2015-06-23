using System;
using System.Collections.Generic;
using System.Linq;
using KinderChat.WorkerRole.SocketServer.Domain;
using StackExchange.Redis;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.Redis
{
    public class RedisDeviceRepository : IDevicesRepository
    {
        private readonly IDatabase _cache;
        private const string KeyPrefix = "rdr_";

        public RedisDeviceRepository(string connectionString)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(connectionString);
            _cache = connection.GetDatabase();
        }

        public List<string> GetDevices(long userId)
        {
            try
            {
                return _cache.ListRange(KeyPrefix + userId.ToString(), flags: CommandFlags.FireAndForget).Select(i => i.ToString()).Distinct().ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public void AddDevice(long userId, string deviceId)
        {
            _cache.ListLeftPush(KeyPrefix + userId.ToString(), deviceId, flags: CommandFlags.FireAndForget);
        }

        public void SetPublicKeyForDevice(string deviceId, byte[] publicKey)
        {
            _cache.StringSet(deviceId, publicKey, flags: CommandFlags.FireAndForget);
        }

        public byte[] GetPublicKeyForDevice(string device)
        {
            return _cache.StringGet(device);
        }
    }
}
