using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.Redis
{
    public static class RedisExtensions
    {
        public static T Get<T>(this IDatabase cache, string key)
        {
            return Deserialize<T>(cache.StringGet(key));
        }
        
        public static void Set(this IDatabase cache, string key, object value)
        {
            cache.StringSet(key, Serialize(value));
        }

        public static async Task<T> GetAsync<T>(this ITransaction cache, string key)
        {
            return Deserialize<T>(await cache.StringGetAsync(key).ConfigureAwait(false));
        }

        public static Task<bool> SetAsync(this ITransaction cache, string key, object value)
        {
            return cache.StringSetAsync(key, Serialize(value));
        }

        /* TODO: BinaryFormatter or Protobuf? */

        public static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        public static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
    }
}
