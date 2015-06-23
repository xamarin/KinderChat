namespace KinderChat.WorkerRole.SocketServer.Domain
{
    public interface IGlobalSessionsRegistry
    {
        void Set(string userId, string serverInstanceId);

        void Remove(string userId, string serverInstanceId);

        string Get(string userId);

        void ClearForInstance(string serverInstanceId);
    }
}
