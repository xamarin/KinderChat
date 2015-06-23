using System.Net.Sockets;

namespace KinderChat.WorkerRole.SocketServer.Domain
{
    public interface ISession
    {
        bool IsAuthenticated { get; }

        string AccessToken { get; }

        string DeviceId { get; }

        long UserId { get; }

        void AssignUser(string accessToken, string deviceId, long userId);
        
        void Send(string eventName, object dto);
    }
}
