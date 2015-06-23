using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KinderChat.ServiceBusShared.Entities;
using KinderChat.WorkerRole.SocketServer.Domain;
using KinderChat.WorkerRole.SocketServer.Infrastructure.Helpers;

namespace KinderChat.WorkerRole.SocketServer.Api.Base.EventManagement
{
    public class ReliableEventManager<TEvent, TEventDto> : IPulsable where TEvent : Event
    {
        private readonly ISessionsServer _server;
        private readonly IUndeliveredEventsRepository<TEvent> _undeliveredEventsRepository;
        private readonly IGlobalSessionsRegistry _sessionsRegistry;
        private readonly IInternalMessageBus _internalMessageBus;
        private readonly Action<TEvent, bool> _eventDeliveryStatusChanged;
        private readonly Func<TEvent, TEventDto> _dtoMapper;
        private readonly ConcurrentDictionary<Guid, TEvent> _pendingEvents = new ConcurrentDictionary<Guid, TEvent>();
        private const string Event = "Event";

        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        public ReliableEventManager(ISessionsServer server,
            IUndeliveredEventsRepository<TEvent> undeliveredEventsRepository,
            IGlobalSessionsRegistry sessionsRegistry,
            IInternalMessageBus internalMessageBus,
            Func<TEvent, TEventDto> dtoMapper, 
            Action<TEvent, bool> eventDeliveryStatusChanged = null)
        {
            _server = server;
            _undeliveredEventsRepository = undeliveredEventsRepository;
            _sessionsRegistry = sessionsRegistry;
            _internalMessageBus = internalMessageBus;
            _dtoMapper = dtoMapper;
            _eventDeliveryStatusChanged = eventDeliveryStatusChanged ?? delegate { };

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

            if (_undeliveredEventsRepository != null)
            {
                _undeliveredEventsRepository.Add(eventObj);
            }

            ISession targetSession;
            if (_server.ActiveSessionsByDeviceId.TryGetValue(eventObj.ReceiverDeviceId, out targetSession))
            {
                _pendingEvents[eventObj.EventId] = eventObj;
                targetSession.Send(Event, eventObj is TEventDto ? (TEventDto)(object)eventObj : _dtoMapper(eventObj));
            }
            else if (_internalMessageBus != null && _sessionsRegistry != null)
            {
                var instanceName = _sessionsRegistry.Get(eventObj.ReceiverDeviceId);
                if (!string.IsNullOrEmpty(instanceName))
                {
                    //user is online but he is attached to another instance
                    //send the event to that server by instanceName (TODO: check if instance is available)
                    _internalMessageBus.Send(eventObj, instanceName);
                }
                else
                {
                    _eventDeliveryStatusChanged(eventObj, false);
                }
            }
        }

        public void AcknowledgeEvent(string fromDeviceId, List<Guid> eventIdList)
        {
            foreach (var id in eventIdList)
            {
                TEvent e;
                _pendingEvents.TryRemove(id, out e);
            }

            foreach (var item in _undeliveredEventsRepository.DeleteAll(fromDeviceId, eventIdList))
            {
                _eventDeliveryStatusChanged(item, true);
            }
        }

        public List<TEvent> GetMissedEvents(string deviceId)
        {
            //CHECK PENDINGS!
            return _undeliveredEventsRepository.GetAll(deviceId);
        }

        public void DeliverMissedEvents(ISession session)
        {
            var missedEvents = GetMissedEvents(session.DeviceId);
            foreach (var missedEvent in missedEvents)
            {
                session.Send(Event, _dtoMapper(missedEvent));
            }
        }

        public void Dispose()
        {
            if (_internalMessageBus != null)
            {
                _internalMessageBus.EventReceived -= OnMessageFromAnotherInstance;
            }
        }
 
        public void HandleTimerTick()
        {
            foreach (var item in _pendingEvents.ToList())
            {
                if (item.Value.CreatedAt.Add(Timeout) <= DateTime.UtcNow)
                {
                    TEvent e;
                    _pendingEvents.TryRemove(item.Key, out e);
                    _eventDeliveryStatusChanged(e, false);
                }
            }
        }

        private void OnMessageFromAnotherInstance(Event e)
        {
            var tevent = e as TEvent;
            if (tevent == null)
                return;

            ISession targetSession;
            if (_server.ActiveSessionsByDeviceId.TryGetValue(e.ReceiverDeviceId, out targetSession))
            {
                _pendingEvents[e.EventId] = tevent;
                targetSession.Send(Event, _dtoMapper(tevent));
            }
            else
            {
                _eventDeliveryStatusChanged(tevent, false);
            }
        }
    }
}
