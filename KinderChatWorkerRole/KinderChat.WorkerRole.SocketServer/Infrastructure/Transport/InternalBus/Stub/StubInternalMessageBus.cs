using System;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;

namespace KinderChat.WorkerRole.SocketServer.Infrastructure.Transport.InternalBus.Stub
{
    /// <summary>
    /// for standalone mode
    /// </summary>
    public class StubInternalMessageBus : IInternalMessageBus
    {
        public void Send(Event e, string targetInstanceName)
        {
        }

        public event Action<Event> EventReceived = delegate { };
    }
}
