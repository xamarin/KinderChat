using System;
using System.Collections.Generic;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;

namespace KinderChat.WorkerRole.SocketServer.Api.Base.EventManagement
{
    public class InstantEventManager<TEvent, TEventDto> : IPulsable where TEvent : Event
    {
        private readonly ISessionsServer _server;
        private readonly IGlobalSessionsRegistry _sessionsRegistry;
        private readonly IInternalMessageBus _internalMessageBus;
        private readonly Func<TEvent, TEventDto> _dtoMapper;
        private const string Event = "Event";

        public InstantEventManager(ISessionsServer server, 
            IGlobalSessionsRegistry sessionsRegistry, 
            IInternalMessageBus internalMessageBus, 
            Func<TEvent, TEventDto> dtoMapper)
        {
            _server = server;
            _sessionsRegistry = sessionsRegistry;
            _internalMessageBus = internalMessageBus;
            _dtoMapper = dtoMapper;

            if (_internalMessageBus != null)
            {
                _internalMessageBus.EventReceived += OnMessageFromAnotherInstance;
            }
        }

        public void DeliverEventToDevices(IEnumerable<string> devices, Func<TEvent> eventObjFactory)
        {
            foreach (var device in devices)
            {
                var eventObj = eventObjFactory();
                eventObj.ReceiverDeviceId = device;
                DeliverEventToDevice(eventObj);
            }
        }

        public void DeliverEventToDevice(TEvent eventObj)
        {
            eventObj.CreatedAt = DateTime.UtcNow;
            if (eventObj.EventId == Guid.Empty)
            {
                eventObj.EventId = Guid.NewGuid();
            }

            ISession targetSession;
            if (_server.ActiveSessionsByDeviceId.TryGetValue(eventObj.ReceiverDeviceId, out targetSession))
            {
                targetSession.Send(Event, eventObj is TEventDto ? (TEventDto)(object)eventObj : _dtoMapper(eventObj));
            }
            else if (_internalMessageBus != null && _sessionsRegistry != null)
            {
                var instanceName = _sessionsRegistry.Get(eventObj.ReceiverDeviceId);
                if (!string.IsNullOrEmpty(instanceName))
                {
                    _internalMessageBus.Send(eventObj, instanceName);
                }
            }
        }


        public void Dispose()
        {
            if (_internalMessageBus != null)
            {
                _internalMessageBus.EventReceived -= OnMessageFromAnotherInstance;
            }
        }

        public void HandleTimerTick() {}

        private void OnMessageFromAnotherInstance(Event e)
        {
            var tevent = e as TEvent;
            if (tevent == null)
                return;

            ISession targetSession;
            if (_server.ActiveSessionsByDeviceId.TryGetValue(e.ReceiverDeviceId, out targetSession))
            {
                targetSession.Send(Event, _dtoMapper(tevent));
            }
        }
    }
}
