using KinderChat.WorkerRole.SocketServer.Domain;
using StackExchange.Redis;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.Redis
{
    public class RedisSessionsRegistry : IGlobalSessionsRegistry
    {
        private readonly IDatabase _cache;
        private const string KeyPrefix = "rsr_";

        public RedisSessionsRegistry(string connectionString)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(connectionString);//set "Allow access only via SSL" to false, we don't need that overhead
            _cache = connection.GetDatabase();
        }

        public void Set(string userId, string serverInstanceId)
        {
            _cache.StringSet(KeyPrefix + userId, serverInstanceId, flags: CommandFlags.FireAndForget);
        }

        public void Remove(string userId, string serverInstanceId)
        {
            /*var transaction = cache.CreateTransaction();
            var instance = await transaction.StringGetAsync(userId).ConfigureAwait(false);
            if (instance == serverInstanceId)
            {
                await transaction.KeyDeleteAsync(userId).ConfigureAwait(false);
            }
            transaction.Execute();*/
            
            //Lock
            var instance = _cache.StringGet(KeyPrefix + userId);
            if (instance == serverInstanceId)
            {
                _cache.KeyDeleteAsync(KeyPrefix + userId, flags: CommandFlags.FireAndForget);
            }
        }

        public string Get(string userId)
        {
            return _cache.StringGet(KeyPrefix + userId);
        }

        public void ClearForInstance(string serverInstanceId)
        {
            //TODO: remove all records with this value
        }
    }
}
