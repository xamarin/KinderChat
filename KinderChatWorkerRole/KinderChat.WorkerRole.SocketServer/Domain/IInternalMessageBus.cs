using System;
using KinderChat.ServiceBusShared.Entities;

namespace KinderChat.WorkerRole.SocketServer.Domain
{
    public interface IInternalMessageBus
    {
        void Send(Event e, string targetInstanceName);

        event Action<Event> EventReceived;
    }
}
