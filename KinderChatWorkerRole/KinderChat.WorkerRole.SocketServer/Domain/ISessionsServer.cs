using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KinderChat.WorkerRole.SocketServer.Domain
{

    public interface ISessionsServer
    {
        ConcurrentDictionary<string, ISession> ActiveSessionsByDeviceId { get; }

        ConcurrentDictionary<long, List<ISession>> ActiveSessionsByUserId { get; }

        void OnSessionAuthenticated(ISession session);

        void InitializeAndRun(string instanceName, int port, int maxConnections);

        void Stop();
    }
}
