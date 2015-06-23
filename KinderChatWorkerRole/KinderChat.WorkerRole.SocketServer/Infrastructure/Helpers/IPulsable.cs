using System;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers
{
    public interface IPulsable : IDisposable
    {
        void HandleTimerTick();
    }
}