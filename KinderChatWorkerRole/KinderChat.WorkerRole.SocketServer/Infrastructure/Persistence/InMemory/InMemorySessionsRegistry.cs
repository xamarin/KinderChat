using KinderChat.WorkerRole.SocketServer.Domain;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Persistence.InMemory
{
    public class InMemorySessionsRegistry : IGlobalSessionsRegistry
    {
        public void Set(string userId, string serverInstanceId)
        {
        }

        public void Remove(string userId, string serverInstanceId)
        {
        }

        public string Get(string userId)
        {
            return null;
        }

        public void ClearForInstance(string serverInstanceId)
        {
        }
    }
}
